using System.Collections.Generic;
using System.Threading.Tasks;
using BookLibraryDataAccessClassLibrary.Models;
using BookLibraryDataAccessClassLibrary.ViewModels;

namespace BookLibraryDataAccessClassLibrary.Interfaces
{
    public interface IAllAuthors
    {
        Task<IEnumerable<Author>> GetAllAuthorsAsync();
        Task<Author> GetByIdAsync(int id);
        Task CreateAsync(Author author);
        Task UpdateAsync(Author author);
        Task DeleteAsync(Author author);
    }
}