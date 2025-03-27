using System.Collections.Generic;
using System.Threading.Tasks;
using BookLibraryDataAccessClassLibrary.Models;
using BookLibraryDataAccessClassLibrary.ViewModels;

namespace BookLibraryDataAccessClassLibrary.Interfaces
{
    public interface IAllAuthors
    {
        Task<IEnumerable<Author>> GetAllAuthorsAsync(CancellationToken cancellationToken);
        Task<Author> GetByIdAsync(int id, CancellationToken cancellationToken);
        Task CreateAsync(Author author, CancellationToken cancellationToken);
        Task UpdateAsync(Author author, CancellationToken cancellationToken);
        Task DeleteAsync(Author author, CancellationToken cancellationToken);
    }
}