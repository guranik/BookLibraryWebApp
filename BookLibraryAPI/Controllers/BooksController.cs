﻿using Microsoft.AspNetCore.Authorization;
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
        public IActionResult SearchBooks(
            string genre = "",
            string author = "",
            string bookName = "",
            int pageNumber = 1,
            int pageSize = 10)
        {
            var book = _bookService.GetByISBN(bookName);

            if (book != null)
            {
                var bookDto = _mapper.Map<BookInfoDto>(book);
                return Ok(bookDto);
            }

            var pagedBooks = _bookService.GetPagedBooks(genre, author, bookName, pageNumber, pageSize);

            if (!pagedBooks.Items.Any())
            {
                bool allIssued = _bookService.AreAllBooksIssued(bookName, int.TryParse(author, out var authorId) ? authorId : 0);
                string message = allIssued
                    ? $"All Books {bookName} by {pagedBooks.Items.FirstOrDefault()?.Author.Name} {pagedBooks.Items.FirstOrDefault()?.Author.Surname} were issued."
                    : "No matches found.";
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
        public IActionResult IssueBook(int bookId, [FromBody] int userId)
        {
             _bookService.IssueBook(bookId);
            var issuedBook = new IssuedBook
            {
                BookId = bookId,
                UserId = userId,
                Issued = DateTime.UtcNow,
                Return = DateTime.UtcNow.AddDays(14)
            };
             _issuedBookService.Create(issuedBook);

            var book = _bookService.GetById(bookId);
            var bookDto = _mapper.Map<BookDto>(book);

            return Ok(bookDto);
        }

        [HttpDelete("return/{issuedBookId}")]
        public IActionResult ReturnBook(int issuedBookId)
        {
            var issuedBook = _issuedBookService.GetById(issuedBookId);
            if (issuedBook == null)
            {
                return NotFound();
            }

            _bookService.ReturnBook(issuedBookId);
            _issuedBookService.Delete(issuedBook);

            return NoContent();
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var book = _bookService.GetById(id);
            if (book == null)
            {
                return NotFound();
            }

            var bookDto = _mapper.Map<BookInfoDto>(book);
            return Ok(bookDto);
        }

        [HttpGet("author/{authorId}")]
        public IActionResult GetByAuthor(int authorId)
        {
            var books = _bookService.GetByAuthor(authorId);
            var bookDtos = _mapper.Map<List<BookDto>>(books);
            return Ok(bookDtos);
        }

        [HttpPost]
        [Authorize(Policy = "AdminOnly")]
        public IActionResult Create([FromBody] BookInfoDto bookDto)
        {
            if (bookDto == null)
            {
                return BadRequest("Book cannot be null.");
            }

            var book = _mapper.Map<Book>(bookDto);
            book.Author = _authorService.GetById(bookDto.AuthorId);
            book.Genre = _genresService.GetById(bookDto.GenreId);
            _bookService.Create(book);
            return CreatedAtAction(nameof(GetById), new { id = book.Id }, bookDto);
        }

        [HttpPut("{id}")]
        [Authorize(Policy = "AdminOnly")]
        public IActionResult Update(int id, [FromBody] BookInfoDto bookDto)
        {
            if (bookDto == null || bookDto.Id != id)
            {
                return BadRequest("Book ID mismatch.");
            }

            var existingBook = _bookService.GetById(id);
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

            _bookService.Update(existingBook);

            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Policy = "AdminOnly")]
        public IActionResult Delete(int id)
        {
            var book = _bookService.GetById(id);
            if (book == null)
            {
                return NotFound();
            }

            _bookService.Delete(book);
            return NoContent();
        }
    }
}