using BookLibraryAPI.Models;

namespace BookLibraryAPI.Interfaces
{
    public interface IAllCountries
    {
        IEnumerable<Country> AllCountries { get; }
        Country GetById(int id);
        void Create(Country country);
        void Update(Country country);
        void Delete(Country country);
    }
}
