using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using BookLibraryDataAccessClassLibrary.Interfaces;
using BookLibraryDataAccessClassLibrary.Models;
using BookLibraryBusinessLogicClassLibrary.DTOs.Countries;

namespace BookLibraryBusinessLogicClassLibrary.Services
{
    public interface ICountryService
    {
        Task<List<CountryDto>> GetAllCountriesAsync(CancellationToken cancellationToken);
        Task<CountryDto> GetCountryByIdAsync(int id, CancellationToken cancellationToken);
        Task CreateCountryAsync(CountryDto countryDto, CancellationToken cancellationToken);
        Task UpdateCountryAsync(CountryDto countryDto, CancellationToken cancellationToken);
        Task DeleteCountryAsync(int id, CancellationToken cancellationToken);
    }

    public class CountryService : ICountryService
    {
        private readonly IAllCountries _countryRepository;
        private readonly IMapper _mapper;

        public CountryService(IAllCountries countryRepository, IMapper mapper)
        {
            _countryRepository = countryRepository;
            _mapper = mapper;
        }

        public async Task<List<CountryDto>> GetAllCountriesAsync(CancellationToken cancellationToken)
        {
            var countries = await _countryRepository.GetAllCountriesAsync(cancellationToken);
            return _mapper.Map<List<CountryDto>>(countries);
        }

        public async Task<CountryDto> GetCountryByIdAsync(int id, CancellationToken cancellationToken)
        {
            var country = await _countryRepository.GetByIdAsync(id, cancellationToken);
            return _mapper.Map<CountryDto>(country);
        }

        public async Task CreateCountryAsync(CountryDto countryDto, CancellationToken cancellationToken)
        {
            var country = _mapper.Map<Country>(countryDto);
            await _countryRepository.CreateAsync(country, cancellationToken);
        }

        public async Task UpdateCountryAsync(CountryDto countryDto, CancellationToken cancellationToken)
        {
            var country = _mapper.Map<Country>(countryDto);
            await _countryRepository.UpdateAsync(country, cancellationToken);
        }

        public async Task DeleteCountryAsync(int id, CancellationToken cancellationToken)
        {
            var country = await _countryRepository.GetByIdAsync(id, cancellationToken);
            if (country != null)
            {
                await _countryRepository.DeleteAsync(country, cancellationToken);
            }
        }
    }
}