using System.Collections.Generic;
using System.Threading.Tasks;
using BookLibraryAPI.Models;

namespace BookLibraryAPI.Interfaces
{
    public interface IAllCountries
    {
        Task<IEnumerable<Country>> GetAllCountriesAsync();
        Task<Country> GetByIdAsync(int id);
        Task CreateAsync(Country country);
        Task UpdateAsync(Country country);
        Task DeleteAsync(Country country);
    }
}