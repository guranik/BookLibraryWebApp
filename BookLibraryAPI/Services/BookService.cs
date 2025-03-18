using System.Collections.Generic;
using System.Linq;
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
        
        public IEnumerable<Book> AllBooks => _context.Books.Include(b => b.Author).Include(b => b.Genre).ToList();

        public PagedList<Book> GetPagedBooks(string genre, string author, string bookName, int pageNumber, int pageSize)
        {
            IQueryable<Book> books = _context.Books.Include(b => b.Author).Include(b => b.Genre);

            if (!string.IsNullOrEmpty(genre))
            {
                books = books.Where(b => b.Genre.Name == genre);
            }

            if (!string.IsNullOrEmpty(author))
            {
                var authorParts = author.Split(' ');
                string firstName = authorParts[0];
                string lastName = authorParts[1];

                books = books.Where(b =>
                    (b.Author.Name.Contains(firstName) || string.IsNullOrEmpty(firstName)) &&
                    (b.Author.Surname.Contains(lastName) || string.IsNullOrEmpty(lastName))
                );
            }

            if (!string.IsNullOrEmpty(bookName))
            {
                books = books.Where(b => b.Title.Equals(bookName));
            }

            var totalCount = books.Count();
            var items = books.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

            return new PagedList<Book>(items, totalCount, pageNumber, pageSize);
        }

        public Book GetById(int id)
        {
            return _context.Books.Include(b => b.Author).Include(b => b.Genre).FirstOrDefault(b => b.Id == id) ??
                throw new InvalidOperationException($"Book with ID {id} not found.");
        }

        public Book GetByISBN(string name)
        {
            return _context.Books.Include(b => b.Author).Include(b => b.Genre)
                .FirstOrDefault(b => b.ISBN.Equals(name));
        }

        public void IssueBook(int bookId)
        {
            var book = _context.Books.FirstOrDefault(b => b.Id == bookId);

            if (book.BookNumber != 0)
            {
                book.BookNumber--;
                _context.SaveChanges();
            }
            else
            {
                throw new NoAvailableBooksException($"There is no books \"{book.Title}\" : \"{book.ISBN}\" left.");
            }
        }

        public void ReturnBook(int bookId)
        {
            var book = _context.Books.FirstOrDefault(b => b.Id == bookId);

                book.BookNumber++;
                _context.SaveChanges();
        }

        public IEnumerable<Book> GetByAuthor(int authorId)
        {
            return _context.Books.Include(b => b.Author).Include(b => b.Genre)
                .Where(b => b.AuthorId == authorId).ToList();
        }

        public void Update(Book book)
        {
            _context.Books.Update(book);
            _context.SaveChanges();
        }

        public void Delete(Book book)
        {
            _context.Books.Remove(book);
            _context.SaveChanges();
        }

        public void Create(Book book)
        {
            _context.Books.Add(book);
            _context.SaveChanges();
        }

        public bool AreAllBooksIssued(string title, int authorId)
        {
            if(_context.Books.Any(b => b.AuthorId == authorId && b.Title == title))
                {
                return _context.Books
                    .Any(b => b.AuthorId == authorId && b.Title == title && b.BookNumber != 0);
            }
            return false;
        }
    }
}