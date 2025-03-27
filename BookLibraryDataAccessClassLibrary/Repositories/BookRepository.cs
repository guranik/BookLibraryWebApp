
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookLibraryDataAccessClassLibrary.Interfaces;
using BookLibraryDataAccessClassLibrary.Models;
using BookLibraryDataAccessClassLibrary.ViewModels;
using BookLibraryDataAccessClassLibrary.Exceptions;
using Microsoft.EntityFrameworkCore;
using System.Threading;

namespace BookLibraryDataAccessClassLibrary.Repositories
{
    public class BookRepository : IAllBooks
    {
        private readonly Db15460Context _context;

        public BookRepository(Db15460Context context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Book>> GetAllBooksAsync(CancellationToken cancellationToken)
        {
            return await _context.Books.Include(b => b.Author).Include(b => b.Genre).ToListAsync(cancellationToken);
        }

        public async Task<PagedList<Book>> GetPagedBooksAsync(string genre, string author, string bookName, int pageNumber, int pageSize, CancellationToken cancellationToken)
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

            var totalCount = await books.CountAsync(cancellationToken);
            var items = await books.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync(cancellationToken);

            return new PagedList<Book>(items, totalCount, pageNumber, pageSize);
        }

        public async Task<Book> GetByIdAsync(int id, CancellationToken cancellationToken)
        {
            return await _context.Books.Include(b => b.Author).Include(b => b.Genre).FirstOrDefaultAsync(b => b.Id == id, cancellationToken)
                ?? throw new InvalidOperationException($"Book with ID {id} not found.");
        }

        public async Task<Book> GetByISBNAsync(string isbn, CancellationToken cancellationToken)
        {
            return await _context.Books.Include(b => b.Author).Include(b => b.Genre)
                .FirstOrDefaultAsync(b => b.ISBN.Equals(isbn), cancellationToken);
        }

        public async Task IssueBookAsync(int bookId, CancellationToken cancellationToken)
        {
            var book = await GetByIdAsync(bookId, cancellationToken);

            if (book.BookNumber > 0)
            {
                book.BookNumber--;
                await _context.SaveChangesAsync(cancellationToken);
            }
            else
            {
                throw new NoAvailableBooksException($"There are no books \"{book.Title}\" : \"{book.ISBN}\" left.");
            }
        }

        public async Task ReturnBookAsync(int bookId, CancellationToken cancellationToken)
        {
            var book = await GetByIdAsync(bookId, cancellationToken);
            if (book != null)
            {
                book.BookNumber++;
                await _context.SaveChangesAsync(cancellationToken);
            }
        }

        public async Task<IEnumerable<Book>> GetByAuthorAsync(int authorId, CancellationToken cancellationToken)
        {
            return await _context.Books.Include(b => b.Author).Include(b => b.Genre)
                .Where(b => b.AuthorId == authorId).ToListAsync(cancellationToken);
        }

        public async Task UpdateAsync(Book book, CancellationToken cancellationToken)
        {
            if (await _context.Books.AnyAsync(b => b.ISBN == book.ISBN && b.Id != book.Id, cancellationToken))
            {
                throw new InvalidOperationException("A book with the same ISBN already exists.");
            }

            _context.Books.Update(book);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task DeleteAsync(Book book, CancellationToken cancellationToken)
        {
            var existingBook = await GetByIdAsync(book.Id, cancellationToken);
            if (existingBook != null)
            {
                _context.Books.Remove(existingBook);
                await _context.SaveChangesAsync(cancellationToken);
            }
            else
            {
                throw new InvalidOperationException($"Book with ID {book.Id} not found.");
            }
        }

        public async Task CreateAsync(Book book, CancellationToken cancellationToken)
        {
            if (await _context.Books.AnyAsync(b => b.ISBN == book.ISBN, cancellationToken))
            {
                throw new InvalidOperationException("A book with this ISBN already exists.");
            }

            await _context.Books.AddAsync(book, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task<bool> AreAllBooksIssuedAsync(string title, string authorName, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(authorName) || string.IsNullOrEmpty(title))
            {
                return false;
            }

            return await _context.Books
                .Where(b => b.Author.Name + " " + b.Author.Surname == authorName && b.Title == title)
                .AllAsync(b => b.BookNumber == 0, cancellationToken);
        }
    }
}