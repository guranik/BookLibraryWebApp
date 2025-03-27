using System.Collections.Generic;
using System.Threading.Tasks;
using BookLibraryDataAccessClassLibrary.Models;
using BookLibraryDataAccessClassLibrary.ViewModels;

namespace BookLibraryDataAccessClassLibrary.Interfaces
{
    public interface IAllIssuedBooks
    {
        Task<PagedList<IssuedBook>> GetPagedIssuedBooksAsync(int page, int pageSize, CancellationToken cancellationToken);
        Task<PagedList<IssuedBook>> GetByUserAsync(int userId, int pageNumber, int pageSize, CancellationToken cancellationToken);
        Task<IssuedBook> GetByIdAsync(int id, CancellationToken cancellationToken);
        Task CreateAsync(IssuedBook issuedBook, CancellationToken cancellationToken);
        Task UpdateAsync(IssuedBook issuedBook, CancellationToken cancellationToken);
        Task DeleteAsync(IssuedBook issuedBook, CancellationToken cancellationToken);
    }
}