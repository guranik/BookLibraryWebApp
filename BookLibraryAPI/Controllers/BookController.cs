using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BookLibraryAPI.Models;
using BookLibraryAPI.Services;
using System.Threading.Tasks;
using BookLibraryAPI.Interfaces;
using System.Runtime.CompilerServices;
using BookLibraryAPI.Middleware;

namespace BookLibraryAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BooksController : ControllerBase
    {
        private readonly IAllBooks _bookService;
        private readonly IAllIssuedBooks _issuedBookService;

        public BooksController(IAllBooks bookService, IAllIssuedBooks issuedBookService)
        {
            _bookService = bookService;
            _issuedBookService = issuedBookService;
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchBooks(
            string genre = "",
            string author = "",
            string bookName = "",
            int pageNumber = 1,
            int pageSize = 10)
        {
            var book = _bookService.GetByISBN(bookName);

            if (book != null)
            {
                return Ok(book);
            }

            var pagedBooks = await Task.Run(() => _bookService.GetPagedBooks(genre, author, bookName, pageNumber, pageSize));

            if (!pagedBooks.Items.Any())
            {
                bool allIssued = _bookService.AreAllBooksIssued(bookName, author != null ? int.Parse(author) : 0);
                string message = allIssued
                    ? $"All Books {bookName} by {pagedBooks.Items.FirstOrDefault()?.Author.Name} {pagedBooks.Items.FirstOrDefault()?.Author.Surname} were issued."
                    : "No matches found.";
                return NotFound(new { Message = message });
            }

            return Ok(pagedBooks);
        }

        [HttpPost("{bookId}/issue")]
        public IActionResult IssueBook(int bookId, [FromBody] int userId)
        {
            var book = _bookService.GetById(bookId);
            if (book.BookNumber > 0)
            {
                book.BookNumber--;
                _bookService.Update(book);

                var issuedBook = new IssuedBook
                {
                    BookId = bookId,
                    UserId = userId,
                    Issued = DateTime.UtcNow,
                    Return = DateTime.UtcNow.AddDays(14)
                };
                _issuedBookService.Create(issuedBook);

                return Ok(issuedBook);
            }
            else
            {
                throw new NoAvailableBooksException($"There are no copies of \"{book.Title}\" available.");
            }
        }

        [HttpDelete("return/{issuedBookId}")]
        public IActionResult ReturnBook(int issuedBookId)
        {
            var issuedBook = _issuedBookService.GetById(issuedBookId);
            if (issuedBook == null)
            {
                return NotFound(new { Message = $"Issued book with ID {issuedBookId} not found." });
            }

            var book = _bookService.GetById(issuedBook.BookId);
            book.BookNumber++;
            _bookService.Update(book);

            _issuedBookService.Delete(issuedBook);

            return NoContent();
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var book = _bookService.GetById(id);
            return Ok(book);
        }

        [HttpGet("author/{id}")]
        public IActionResult GetByAuthor(int authorId)
        {
            var book = _bookService.GetByAuthor(authorId);
            return Ok(book);
        }

        [HttpPost]
        [Authorize(Policy = "AdminOnly")]
        public IActionResult Create([FromBody] Book book)
        {
            _bookService.Create(book);
            return CreatedAtAction(nameof(GetById), new { id = book.Id }, book);
        }

        [HttpPut("{id}")]
        [Authorize(Policy = "AdminOnly")]
        public IActionResult Update(int id, [FromBody] Book book)
        {
            if (id != book.Id)
            {
                return BadRequest("Book ID mismatch.");
            }

            _bookService.Update(book);
            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Policy = "AdminOnly")]
        public IActionResult Delete(int id)
        {
            var book = _bookService.GetById(id);
            _bookService.Delete(book);
            return NoContent();
        }
    }
}