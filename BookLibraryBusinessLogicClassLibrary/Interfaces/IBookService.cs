using BookLibraryDataAccessClassLibrary.Models;
using BookLibraryBusinessLogicClassLibrary.DTOs.Books;
using BookLibraryBusinessLogicClassLibrary.DTOs.PagedResult;

namespace BookLibraryBusinessLogicClassLibrary.Interfaces
{
    public interface IBookService
    {
        Task<PagedBooksDto> GetPagedBooksAsync(string genre, string author, string bookName, int pageNumber, int pageSize, CancellationToken cancellationToken);
        Task<BookInfoDto> GetByIdAsync(int id, CancellationToken cancellationToken);
        Task<BookInfoDto> GetByISBNAsync(string isbn, CancellationToken cancellationToken);
        Task IssueBookAsync(int bookId, int userId, CancellationToken cancellationToken);
        Task ReturnBookAsync(int issuedBookId, CancellationToken cancellationToken);
        Task CreateAsync(BookInfoDto bookDto, CancellationToken cancellationToken);
        Task UpdateAsync(int id, BookInfoDto bookDto, CancellationToken cancellationToken);
        Task DeleteAsync(int id, CancellationToken cancellationToken);
        Task<bool> AreAllBooksIssuedAsync(string title, string authorName, CancellationToken cancellationToken);
        Task<List<BookDto>> GetByAuthorAsync(int authorId, CancellationToken cancellationToken);
    }
}