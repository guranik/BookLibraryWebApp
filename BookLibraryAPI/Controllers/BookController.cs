using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BookLibraryAPI.Models;
using BookLibraryAPI.Services;
using System.Threading.Tasks;
using BookLibraryAPI.Interfaces;
using System.Runtime.CompilerServices;

namespace BookLibraryAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BooksController : ControllerBase
    {
        private readonly IAllBooks _bookService;

        public BooksController(IAllBooks bookService)
        {
            _bookService = bookService;
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
                // Return the book details as JSON
                return Ok(book);
            }

            var pagedBooks = _bookService.GetPagedBooks(genre, author, bookName, pageNumber, pageSize);

            if (!pagedBooks.Items.Any())
            {
                bool allIssued = _bookService.AreAllBooksIssued(bookName, author != null ? int.Parse(author) : 0);
                string message = allIssued
                    ? $"All Books {bookName} by {pagedBooks.Items.FirstOrDefault()?.Author.Name} {pagedBooks.Items.FirstOrDefault()?.Author.Surname} were issued."
                    : "No matched books found.";
                return NotFound(new { Message = message });
            }

            return Ok(pagedBooks);
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var book = _bookService.GetById(id);
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