using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookLibraryDataAccessClassLibrary.Interfaces;
using BookLibraryDataAccessClassLibrary.Models;
using BookLibraryDataAccessClassLibrary.ViewModels;
using BookLibraryDataAccessClassLibrary.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace BookLibraryDataAccessClassLibrary.Repositories
{
    public class BookRepository : IAllBooks
    {
        private readonly Db15460Context _context;

        public BookRepository(Db15460Context context)
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
            var book = await GetByIdAsync(bookId);

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
            var book = await GetByIdAsync(bookId);
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
            if (await _context.Books.AnyAsync(b => b.ISBN == book.ISBN && b.Id != book.Id))
            {
                throw new InvalidOperationException("A book with the same ISBN already exists.");
            }

            _context.Books.Update(book);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Book book)
        {
            var existingBook = await GetByIdAsync(book.Id);
            if (existingBook != null)
            {
                _context.Books.Remove(existingBook);
                await _context.SaveChangesAsync();
            }
            else
            {
                throw new InvalidOperationException($"Book with ID {book.Id} not found.");
            }
        }

        public async Task CreateAsync(Book book)
        {
            if (await _context.Books.AnyAsync(b => b.ISBN == book.ISBN))
            {
                throw new InvalidOperationException("A book with this ISBN already exists.");
            }

            await _context.Books.AddAsync(book);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> AreAllBooksIssuedAsync(string title, string authorName)
        {
            if (string.IsNullOrEmpty(authorName) || string.IsNullOrEmpty(title))
            {
                return false;
            }

            return await _context.Books
                .Where(b => b.Author.Name + " " + b.Author.Surname == authorName && b.Title == title)
                .AllAsync(b => b.BookNumber == 0);
        }
    }
}