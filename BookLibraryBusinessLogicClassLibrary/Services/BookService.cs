using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using BookLibraryDataAccessClassLibrary.Interfaces;
using BookLibraryDataAccessClassLibrary.Models;
using BookLibraryBusinessLogicClassLibrary.DTOs.Books;
using BookLibraryBusinessLogicClassLibrary.DTOs.PagedResult;

namespace BookLibraryBusinessLogicClassLibrary.Services
{
    public interface IBookService
    {
        Task<PagedBooksDto> GetPagedBooksAsync(string genre, string author, string bookName, int pageNumber, int pageSize);
        Task<BookInfoDto> GetByIdAsync(int id);
        Task<Book> GetByISBNAsync(string isbn);
        Task IssueBookAsync(int bookId, int userId);
        Task ReturnBookAsync(int issuedBookId);
        Task CreateAsync(BookInfoDto bookDto);
        Task UpdateAsync(int id, BookInfoDto bookDto);
        Task DeleteAsync(int id);
        Task<bool> AreAllBooksIssuedAsync(string title, string authorName);
        Task<List<BookDto>> GetByAuthorAsync(int authorId);
    }

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

        public async Task<PagedBooksDto> GetPagedBooksAsync(string genre, string author, string bookName, int pageNumber, int pageSize)
        {
            var pagedBooks = await _bookRepository.GetPagedBooksAsync(genre, author, bookName, pageNumber, pageSize);
            var bookDtos = _mapper.Map<List<BookDto>>(pagedBooks.Items);
            return new PagedBooksDto
            {
                Items = bookDtos,
                TotalPages = pagedBooks.TotalPages,
                CurrentPage = pagedBooks.PageNumber
            };
        }

        public async Task<BookInfoDto> GetByIdAsync(int id)
        {
            var book = await _bookRepository.GetByIdAsync(id);
            return _mapper.Map<BookInfoDto>(book);
        }

        public async Task<Book> GetByISBNAsync(string isbn)
        {
            return await _bookRepository.GetByISBNAsync(isbn);
        }

        public async Task IssueBookAsync(int bookId, int userId)
        {
            await _bookRepository.IssueBookAsync(bookId);
            var issuedBook = new IssuedBook
            {
                BookId = bookId,
                UserId = userId,
                Issued = DateTime.UtcNow,
                Return = DateTime.UtcNow.AddDays(14)
            };
            try
            {
                await _issuedBookService.CreateAsync(issuedBook);
            }
            catch(Exception ex)
            {
                await _bookRepository.ReturnBookAsync(bookId);
            }
        }

        public async Task ReturnBookAsync(int issuedBookId)
        {
            var issuedBook = await _issuedBookService.GetByIdAsync(issuedBookId);
            await _issuedBookService.DeleteAsync(issuedBook);
            await _bookRepository.ReturnBookAsync(issuedBookId);
        }

        public async Task CreateAsync(BookInfoDto bookDto)
        {
            var book = _mapper.Map<Book>(bookDto);
            book.Author = await _authorService.GetByIdAsync(bookDto.AuthorId);
            book.Genre = await _genresService.GetByIdAsync(bookDto.GenreId);
            await _bookRepository.CreateAsync(book);
        }

        public async Task UpdateAsync(int id, BookInfoDto bookDto)
        {
            var existingBook = await _bookRepository.GetByIdAsync(id);
            existingBook.Title = bookDto.Title;
            existingBook.Description = bookDto.Description;
            existingBook.BookNumber = bookDto.BookNumber;
            existingBook.ISBN = bookDto.ISBN;
            existingBook.AuthorId = bookDto.AuthorId;
            existingBook.GenreId = bookDto.GenreId;

            await _bookRepository.UpdateAsync(existingBook);
        }

        public async Task DeleteAsync(int id)
        {
            var book = await _bookRepository.GetByIdAsync(id);
            await _bookRepository.DeleteAsync(book);
        }

        public async Task<bool> AreAllBooksIssuedAsync(string title, string authorName)
        {
            return await _bookRepository.AreAllBooksIssuedAsync(title, authorName);
        }

        public async Task<List<BookDto>> GetByAuthorAsync(int authorId)
        {
            var books = await _bookRepository.GetByAuthorAsync(authorId);
            return _mapper.Map<List<BookDto>>(books);
        }
    }
}