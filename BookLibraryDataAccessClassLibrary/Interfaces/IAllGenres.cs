using System.Collections.Generic;
using System.Threading.Tasks;
using BookLibraryDataAccessClassLibrary.Models;

namespace BookLibraryDataAccessClassLibrary.Interfaces
{
    public interface IAllGenres
    {
        Task<IEnumerable<Genre>> GetAllGenresAsync(CancellationToken cancellationToken);
        Task<Genre> GetByIdAsync(int id, CancellationToken cancellationToken);
        Task CreateAsync(Genre genre, CancellationToken cancellationToken);
        Task UpdateAsync(Genre genre, CancellationToken cancellationToken);
        Task DeleteAsync(Genre genre, CancellationToken cancellationToken);
    }
}