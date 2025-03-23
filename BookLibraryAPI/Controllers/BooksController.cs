using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BookLibraryAPI.Interfaces;
using BookLibraryAPI.DTOs.Books;
using BookLibraryAPI.DTOs.PagedResult;
using System.Collections.Generic;
using System.Threading.Tasks;
using BookLibraryAPI.Services;

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
            int pageSize = 10)
        {
            var pagedBooksDto = await _bookService.GetPagedBooksAsync(genre, author, bookName, pageNumber, pageSize);
            if (pagedBooksDto.Items.Count == 0)
            {
                bool allIssued = await _bookService.AreAllBooksIssuedAsync(bookName, author);
                string message = allIssued
                    ? $"Все книги '{bookName}' автора {author} выданы."
                    : "Совпадений не найдено.";
                return NotFound(new { Message = message });
            }
            return Ok(pagedBooksDto);
        }

        [HttpPost("{bookId}/issue")]
        public async Task<IActionResult> IssueBook(int bookId, [FromBody] int userId)
        {
            await _bookService.IssueBookAsync(bookId, userId);
            var bookDto = await _bookService.GetByIdAsync(bookId);
            return Ok(bookDto);
        }

        [HttpDelete("return/{issuedBookId}")]
        public async Task<IActionResult> ReturnBook(int issuedBookId)
        {
            await _bookService.ReturnBookAsync(issuedBookId);
            return NoContent();
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var bookDto = await _bookService.GetByIdAsync(id);
            if (bookDto == null)
            {
                return NotFound();
            }
            return Ok(bookDto);
        }

        [HttpGet("author/{authorId}")]
        public async Task<IActionResult> GetByAuthor(int authorId)
        {
            var bookDtos = await _bookService.GetByAuthorAsync(authorId);
            return Ok(bookDtos);
        }

        [HttpPost]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> Create([FromBody] BookInfoDto bookDto)
        {
            if (bookDto == null)
            {
                return BadRequest("Книга не может быть пустой.");
            }
            await _bookService.CreateAsync(bookDto);
            return CreatedAtAction(nameof(GetById), new { id = bookDto.Id }, bookDto);
        }

        [HttpPut("{id}")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> Update(int id, [FromBody] BookInfoDto bookDto)
        {
            if (bookDto == null || bookDto.Id != id)
            {
                return BadRequest("Несоответствие ID книги.");
            }
            await _bookService.UpdateAsync(id, bookDto);
            return NoContent();
        }

        [HttpPost("{id}/upload-cover")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> UploadCover(int id, IFormFile file)
        {
            // Upload cover logic
            return Ok(new { Message = "Изображение успешно загружено." });
        }

        [HttpDelete("{id}")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> Delete(int id)
        {
            await _bookService.DeleteAsync(id);
            return NoContent();
        }
    }
}