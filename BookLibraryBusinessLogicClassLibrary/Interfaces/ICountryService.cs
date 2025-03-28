using BookLibraryBusinessLogicClassLibrary.DTOs.Countries;

namespace BookLibraryBusinessLogicClassLibrary.Interfaces
{
    public interface ICountryService
    {
        Task<List<CountryDto>> GetAllCountriesAsync(CancellationToken cancellationToken);
        Task<CountryDto> GetCountryByIdAsync(int id, CancellationToken cancellationToken);
        Task CreateCountryAsync(CountryDto countryDto, CancellationToken cancellationToken);
        Task UpdateCountryAsync(CountryDto countryDto, CancellationToken cancellationToken);
        Task DeleteCountryAsync(int id, CancellationToken cancellationToken);
    }
}