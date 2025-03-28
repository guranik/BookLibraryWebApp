using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using BookLibraryDataAccessClassLibrary.Interfaces;
using BookLibraryDataAccessClassLibrary.Models;
using BookLibraryBusinessLogicClassLibrary.DTOs.Books;
using BookLibraryBusinessLogicClassLibrary.DTOs.PagedResult;
using BookLibraryBusinessLogicClassLibrary.Interfaces;
using System.Threading;
using BookLibraryBusinessLogicClassLibrary.Exceptions;

namespace BookLibraryBusinessLogicClassLibrary.Services
{

    public class BookService : IBookService
    {
        private readonly IAllBooks _bookRepository;
        private readonly IAllAuthors _authorService;
        private readonly IAllGenres _genresService;
        private readonly IAllIssuedBooks _issuedBookService;
        private readonly IMapper _mapper;

        public BookService(IAllBooks bookRepository, IAllAuthors authorService, IAllGenres genresService, IAllIssuedBooks issuedBookService, IMapper mapper)
        {
            _bookRepository = bookRepository;
            _authorService = authorService;
            _genresService = genresService;
            _issuedBookService = issuedBookService;
            _mapper = mapper;
        }

        public async Task<PagedBooksDto> GetPagedBooksAsync(string genre, string author, string bookName, int pageNumber, int pageSize, CancellationToken cancellationToken)
        {
            var pagedBooks = await _bookRepository.GetPagedBooksAsync(genre, author, bookName, pageNumber, pageSize, cancellationToken);
            if (pagedBooks.Items.Count == 0)
            {
                bool allIssued = await AreAllBooksIssuedAsync(bookName, author, cancellationToken);
                string message = allIssued
                    ? $"Все книги '{bookName}' автора {author} выданы."
                    : "Совпадений не найдено.";
                throw new NotFoundException(message);
            }

            var bookDtos = _mapper.Map<List<BookDto>>(pagedBooks.Items);
            return new PagedBooksDto
            {
                Items = bookDtos,
                TotalPages = pagedBooks.TotalPages,
                CurrentPage = pagedBooks.PageNumber
            };
        }

        public async Task<BookInfoDto> GetByIdAsync(int id, CancellationToken cancellationToken)
        {
            var book = await _bookRepository.GetByIdAsync(id, cancellationToken);
            return _mapper.Map<BookInfoDto>(book);
        }

        public async Task<Book> GetByISBNAsync(string isbn, CancellationToken cancellationToken)
        {
            return await _bookRepository.GetByISBNAsync(isbn, cancellationToken);
        }

        public async Task IssueBookAsync(int bookId, int userId, CancellationToken cancellationToken)
        {
            await _bookRepository.IssueBookAsync(bookId, cancellationToken);
            var issuedBook = new IssuedBook
            {
                BookId = bookId,
                UserId = userId,
                Issued = DateTime.UtcNow,
                Return = DateTime.UtcNow.AddDays(14)
            };
            try
            {
                await _issuedBookService.CreateAsync(issuedBook, cancellationToken);
            }
            catch (Exception ex)
            {
                await _bookRepository.ReturnBookAsync(bookId, cancellationToken);
            }
        }

        public async Task ReturnBookAsync(int issuedBookId, CancellationToken cancellationToken)
        {
            var issuedBook = await _issuedBookService.GetByIdAsync(issuedBookId, cancellationToken);
            await _issuedBookService.DeleteAsync(issuedBook, cancellationToken);
            await _bookRepository.ReturnBookAsync(issuedBookId, cancellationToken);
        }

        public async Task CreateAsync(BookInfoDto bookDto, CancellationToken cancellationToken)
        {
            var book = _mapper.Map<Book>(bookDto);
            book.Author = await _authorService.GetByIdAsync(bookDto.AuthorId, cancellationToken);
            book.Genre = await _genresService.GetByIdAsync(bookDto.GenreId, cancellationToken);
            await _bookRepository.CreateAsync(book, cancellationToken);
        }

        public async Task UpdateAsync(int id, BookInfoDto bookDto, CancellationToken cancellationToken)
        {
            var existingBook = await _bookRepository.GetByIdAsync(id, cancellationToken);
            existingBook.Title = bookDto.Title;
            existingBook.Description = bookDto.Description;
            existingBook.BookNumber = bookDto.BookNumber;
            existingBook.ISBN = bookDto.ISBN;
            existingBook.AuthorId = bookDto.AuthorId;
            existingBook.GenreId = bookDto.GenreId;

            await _bookRepository.UpdateAsync(existingBook, cancellationToken);
        }

        public async Task DeleteAsync(int id, CancellationToken cancellationToken)
        {
            var book = await _bookRepository.GetByIdAsync(id, cancellationToken);
            await _bookRepository.DeleteAsync(book, cancellationToken);
        }

        public async Task<bool> AreAllBooksIssuedAsync(string title, string authorName, CancellationToken cancellationToken)
        {
            return await _bookRepository.AreAllBooksIssuedAsync(title, authorName, cancellationToken);
        }

        public async Task<List<BookDto>> GetByAuthorAsync(int authorId, CancellationToken cancellationToken)
        {
            var books = await _bookRepository.GetByAuthorAsync(authorId, cancellationToken);
            return _mapper.Map<List<BookDto>>(books);
        }
    }
}