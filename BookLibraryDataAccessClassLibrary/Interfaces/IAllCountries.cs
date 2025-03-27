using System.Collections.Generic;
using System.Threading.Tasks;
using BookLibraryDataAccessClassLibrary.Models;

namespace BookLibraryDataAccessClassLibrary.Interfaces
{
    public interface IAllCountries
    {
        Task<IEnumerable<Country>> GetAllCountriesAsync(CancellationToken cancellationToken);
        Task<Country> GetByIdAsync(int id, CancellationToken cancellationToken);
        Task CreateAsync(Country country, CancellationToken cancellationToken);
        Task UpdateAsync(Country country, CancellationToken cancellationToken);
        Task DeleteAsync(Country country, CancellationToken cancellationToken);
    }
}