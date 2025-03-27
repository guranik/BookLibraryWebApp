using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BookLibraryDataAccessClassLibrary.Interfaces;
using BookLibraryBusinessLogicClassLibrary.DTOs.Books;
using BookLibraryBusinessLogicClassLibrary.DTOs.PagedResult;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using BookLibraryBusinessLogicClassLibrary.Services;

namespace BookLibraryAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BooksController : ControllerBase
    {
        private readonly IBookService _bookService;

        public BooksController(IBookService bookService)
        {
            _bookService = bookService;
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchBooks(
            string genre = "",
            string author = "",
            string bookName = "",
            int pageNumber = 1,
            int pageSize = 10,
            CancellationToken cancellationToken = default)
        {
            var pagedBooksDto = await _bookService.GetPagedBooksAsync(genre, author, bookName, pageNumber, pageSize, cancellationToken);
            if (pagedBooksDto.Items.Count == 0)
            {
                bool allIssued = await _bookService.AreAllBooksIssuedAsync(bookName, author, cancellationToken);
                string message = allIssued
                    ? $"Все книги '{bookName}' автора {author} выданы."
                    : "Совпадений не найдено.";
                return NotFound(new { Message = message });
            }
            return Ok(pagedBooksDto);
        }

        [HttpPost("{bookId}/issue")]
        public async Task<IActionResult> IssueBook(int bookId, [FromBody] int userId, CancellationToken cancellationToken = default)
        {
            await _bookService.IssueBookAsync(bookId, userId, cancellationToken);
            var bookDto = await _bookService.GetByIdAsync(bookId, cancellationToken);
            return Ok(bookDto);
        }

        [HttpDelete("return/{issuedBookId}")]
        public async Task<IActionResult> ReturnBook(int issuedBookId, CancellationToken cancellationToken = default)
        {
            await _bookService.ReturnBookAsync(issuedBookId, cancellationToken);
            return NoContent();
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken = default)
        {
            var bookDto = await _bookService.GetByIdAsync(id, cancellationToken);
            if (bookDto == null)
            {
                return NotFound();
            }
            return Ok(bookDto);
        }

        [HttpGet("author/{authorId}")]
        public async Task<IActionResult> GetByAuthor(int authorId, CancellationToken cancellationToken = default)
        {
            var bookDtos = await _bookService.GetByAuthorAsync(authorId, cancellationToken);
            return Ok(bookDtos);
        }

        [HttpPost]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> Create([FromBody] BookInfoDto bookDto, CancellationToken cancellationToken = default)
        {
            if (bookDto == null)
            {
                return BadRequest("Книга не может быть пустой.");
            }
            await _bookService.CreateAsync(bookDto, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = bookDto.Id }, bookDto);
        }

        [HttpPut("{id}")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> Update(int id, [FromBody] BookInfoDto bookDto, CancellationToken cancellationToken = default)
        {
            if (bookDto == null || bookDto.Id != id)
            {
                return BadRequest("Несоответствие ID книги.");
            }
            await _bookService.UpdateAsync(id, bookDto, cancellationToken);
            return NoContent();
        }

        [HttpPost("{id}/upload-cover")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> UploadCover(int id, IFormFile file, CancellationToken cancellationToken = default)
        {
            // Upload cover logic
            return Ok(new { Message = "Изображение успешно загружено." });
        }

        [HttpDelete("{id}")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken = default)
        {
            await _bookService.DeleteAsync(id, cancellationToken);
            return NoContent();
        }
    }
}