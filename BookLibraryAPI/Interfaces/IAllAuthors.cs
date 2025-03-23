using System.Collections.Generic;
using System.Threading.Tasks;
using BookLibraryAPI.Models;
using BookLibraryAPI.ViewModels;

namespace BookLibraryAPI.Interfaces
{
    public interface IAllAuthors
    {
        Task<IEnumerable<Author>> GetAllAuthorsAsync();
        Task<IEnumerable<Author>> GetSortedAuthorsAsync();
        Task<PagedList<Author>> GetPagedAuthorsAsync(int page, int pageSize);
        Task<Author> GetByIdAsync(int id);
        Task CreateAsync(Author author);
        Task UpdateAsync(Author author);
        Task DeleteAsync(Author author);
    }
}