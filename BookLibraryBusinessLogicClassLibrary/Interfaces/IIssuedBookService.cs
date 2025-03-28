using BookLibraryBusinessLogicClassLibrary.DTOs.IssuedBooks;
using BookLibraryBusinessLogicClassLibrary.DTOs.PagedResult;

namespace BookLibraryBusinessLogicClassLibrary.Interfaces
{
    public interface IIssuedBookService
    {
        Task<PagedIssuedBooksDto> GetByUserAsync(int userId, int pageNumber, int pageSize, CancellationToken cancellationToken);
        Task<IssuedBookDto> GetByIdAsync(int id, CancellationToken cancellationToken);
        Task CreateAsync(IssuedBookDto issuedBookDto, CancellationToken cancellationToken);
        Task UpdateAsync(int id, IssuedBookDto issuedBookDto, CancellationToken cancellationToken);
        Task DeleteAsync(int id, CancellationToken cancellationToken);
    }
}