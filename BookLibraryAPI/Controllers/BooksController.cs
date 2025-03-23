using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BookLibraryAPI.Interfaces;
using BookLibraryAPI.Models;
using BookLibraryAPI.DTOs.Books;
using BookLibraryAPI.DTOs.PagedResult;
using AutoMapper;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookLibraryAPI.Services;

namespace BookLibraryAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BooksController : ControllerBase
    {
        private readonly IAllBooks _bookService;
        private readonly IAllAuthors _authorService;
        private readonly IAllGenres _genresService;
        private readonly IAllIssuedBooks _issuedBookService;
        private readonly IMapper _mapper;

        public BooksController(IAllBooks bookService, IAllAuthors authorService, IAllGenres genresService, IAllIssuedBooks issuedBookService, IMapper mapper)
        {
            _bookService = bookService;
            _genresService = genresService;
            _authorService = authorService;
            _issuedBookService = issuedBookService;
            _mapper = mapper;
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchBooks(
            string genre = "",
            string author = "",
            string bookName = "",
            int pageNumber = 1,
            int pageSize = 10)
        {
            var book = await _bookService.GetByISBNAsync(bookName);

            if (book != null)
            {
                var bookDto = _mapper.Map<BookInfoDto>(book);
                return Ok(bookDto);
            }

            var pagedBooks = await _bookService.GetPagedBooksAsync(genre, author, bookName, pageNumber, pageSize);

            if (!pagedBooks.Items.Any())
            {
                bool allIssued = await _bookService.AreAllBooksIssuedAsync(bookName, author);
                string message = allIssued
                    ? $"Все книги '{bookName}' автора {author} выданы."
                    : "Совпадений не найдено.";
                return NotFound(new { Message = message });
            }

            var bookDtos = _mapper.Map<List<BookDto>>(pagedBooks.Items);
            var pagedBooksDto = new PagedBooksDto
            {
                Items = bookDtos,
                TotalPages = pagedBooks.TotalPages,
                CurrentPage = pagedBooks.PageNumber
            };
            return Ok(pagedBooksDto);
        }

        [HttpPost("{bookId}/issue")]
        public async Task<IActionResult> IssueBook(int bookId, [FromBody] int userId)
        {
            await _bookService.IssueBookAsync(bookId);
            var issuedBook = new IssuedBook
            {
                BookId = bookId,
                UserId = userId,
                Issued = DateTime.UtcNow,
                Return = DateTime.UtcNow.AddDays(14)
            };
            await _issuedBookService.CreateAsync(issuedBook);

            var book = await _bookService.GetByIdAsync(bookId);
            var bookDto = _mapper.Map<BookDto>(book);

            return Ok(bookDto);
        }

        [HttpDelete("return/{issuedBookId}")]
        public async Task<IActionResult> ReturnBook(int issuedBookId)
        {
            var issuedBook = await _issuedBookService.GetByIdAsync(issuedBookId);
            if (issuedBook == null)
            {
                return NotFound();
            }

            await _bookService.ReturnBookAsync(issuedBookId);
            await _issuedBookService.DeleteAsync(issuedBook);

            return NoContent();
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var book = await _bookService.GetByIdAsync(id);
            if (book == null)
            {
                return NotFound();
            }

            var bookDto = _mapper.Map<BookInfoDto>(book);
            return Ok(bookDto);
        }

        [HttpGet("author/{authorId}")]
        public async Task<IActionResult> GetByAuthor(int authorId)
        {
            var books = await _bookService.GetByAuthorAsync(authorId);
            var bookDtos = _mapper.Map<List<BookDto>>(books);
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

            var book = _mapper.Map<Book>(bookDto);
            book.Author = await _authorService.GetByIdAsync(bookDto.AuthorId);
            book.Genre = await _genresService.GetByIdAsync(bookDto.GenreId);
            await _bookService.CreateAsync(book);
            return CreatedAtAction(nameof(GetById), new { id = book.Id }, bookDto);
        }

        [HttpPut("{id}")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> Update(int id, [FromBody] BookInfoDto bookDto)
        {
            if (bookDto == null || bookDto.Id != id)
            {
                return BadRequest("Несоответствие ID книги.");
            }

            var existingBook = await _bookService.GetByIdAsync(id);
            if (existingBook == null)
            {
                return NotFound();
            }

            existingBook.Title = bookDto.Title;
            existingBook.Description = bookDto.Description;
            existingBook.BookNumber = bookDto.BookNumber;
            existingBook.ISBN = bookDto.ISBN;
            existingBook.AuthorId = bookDto.AuthorId;
            existingBook.GenreId = bookDto.GenreId;

            await _bookService.UpdateAsync(existingBook);

            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> Delete(int id)
        {
            var book = await _bookService.GetByIdAsync(id);
            if (book == null)
            {
                return NotFound();
            }

            await _bookService.DeleteAsync(book);
            return NoContent();
        }
    }
}