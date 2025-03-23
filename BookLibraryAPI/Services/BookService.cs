using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookLibraryAPI.Interfaces;
using BookLibraryAPI.Models;
using BookLibraryAPI.ViewModels;
using BookLibraryAPI.Middleware;
using Microsoft.EntityFrameworkCore;

namespace BookLibraryAPI.Services
{
    public class BookService : IAllBooks
    {
        private readonly Db15460Context _context;

        public BookService(Db15460Context context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Book>> GetAllBooksAsync()
        {
            return await _context.Books.Include(b => b.Author).Include(b => b.Genre).ToListAsync();
        }

        public async Task<PagedList<Book>> GetPagedBooksAsync(string genre, string author, string bookName, int pageNumber, int pageSize)
        {
            IQueryable<Book> books = _context.Books.Where(b => b.BookNumber > 0).Include(b => b.Author).Include(b => b.Genre);

            if (!string.IsNullOrEmpty(genre))
            {
                books = books.Where(b => b.Genre.Name == genre);
            }

            if (!string.IsNullOrEmpty(author))
            {
                var authorParts = author.Split(' ');
                string firstName = authorParts[0];
                string lastName = authorParts.Length > 1 ? authorParts[1] : string.Empty;

                books = books.Where(b =>
                    (b.Author.Name.Contains(firstName) || string.IsNullOrEmpty(firstName)) &&
                    (b.Author.Surname.Contains(lastName) || string.IsNullOrEmpty(lastName))
                );
            }

            if (!string.IsNullOrEmpty(bookName))
            {
                books = books.Where(b => b.Title.Equals(bookName));
            }
            
            var totalCount = await books.CountAsync();
            var items = await books.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();

            return new PagedList<Book>(items, totalCount, pageNumber, pageSize);
        }

        public async Task<Book> GetByIdAsync(int id)
        {
            return await _context.Books.Include(b => b.Author).Include(b => b.Genre).FirstOrDefaultAsync(b => b.Id == id)
                ?? throw new InvalidOperationException($"Book with ID {id} not found.");
        }

        public async Task<Book> GetByISBNAsync(string isbn)
        {
            return await _context.Books.Include(b => b.Author).Include(b => b.Genre)
                .FirstOrDefaultAsync(b => b.ISBN.Equals(isbn));
        }

        public async Task IssueBookAsync(int bookId)
        {
            var book = await _context.Books.FirstOrDefaultAsync(b => b.Id == bookId);

            if (book.BookNumber > 0)
            {
                book.BookNumber--;
                await _context.SaveChangesAsync();
            }
            else
            {
                throw new NoAvailableBooksException($"There are no books \"{book.Title}\" : \"{book.ISBN}\" left.");
            }
        }

        public async Task ReturnBookAsync(int bookId)
        {
            var book = await _context.Books.FirstOrDefaultAsync(b => b.Id == bookId);
            if (book != null)
            {
                book.BookNumber++;
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Book>> GetByAuthorAsync(int authorId)
        {
            return await _context.Books.Include(b => b.Author).Include(b => b.Genre)
                .Where(b => b.AuthorId == authorId).ToListAsync();
        }

        public async Task UpdateAsync(Book book)
        {
            _context.Books.Update(book);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Book book)
        {
            _context.Books.Remove(book);
            await _context.SaveChangesAsync();
        }

        public async Task CreateAsync(Book book)
        {
            await _context.Books.AddAsync(book);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> AreAllBooksIssuedAsync(string title, string authorName)
        {
            return await _context.Books
                .Where(b => b.Author.Name + " " + b.Author.Surname == authorName && b.Title == title)
                .AllAsync(b => b.BookNumber == 0);
        }
    }
}