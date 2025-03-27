using System.Collections.Generic;
using System.Threading.Tasks;
using BookLibraryDataAccessClassLibrary.Models;

namespace BookLibraryDataAccessClassLibrary.Interfaces
{
    public interface IAllGenres
    {
        Task<IEnumerable<Genre>> GetAllGenresAsync();
        Task<Genre> GetByIdAsync(int id);
        Task CreateAsync(Genre genre);
        Task UpdateAsync(Genre genre);
        Task DeleteAsync(Genre genre);
    }
}