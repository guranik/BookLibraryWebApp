using System.Collections.Generic;
using System.Threading.Tasks;
using BookLibraryDataAccessClassLibrary.Models;
using BookLibraryDataAccessClassLibrary.ViewModels;

namespace BookLibraryDataAccessClassLibrary.Interfaces
{
    public interface IAllIssuedBooks
    {
        Task<PagedList<IssuedBook>> GetPagedIssuedBooksAsync(int page, int pageSize);
        Task<PagedList<IssuedBook>> GetByUserAsync(int userId, int pageNumber, int pageSize);
        Task<IssuedBook> GetByIdAsync(int id);
        Task CreateAsync(IssuedBook issuedBook);
        Task UpdateAsync(IssuedBook issuedBook);
        Task DeleteAsync(IssuedBook issuedBook);
    }
}