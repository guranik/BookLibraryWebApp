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
        Task<List<CountryDto>> GetAllCountriesAsync();
        Task<CountryDto> GetCountryByIdAsync(int id);
        Task CreateCountryAsync(CountryDto countryDto);
        Task UpdateCountryAsync(CountryDto countryDto);
        Task DeleteCountryAsync(int id);
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

        public async Task<List<CountryDto>> GetAllCountriesAsync()
        {
            var countries = await _countryRepository.GetAllCountriesAsync();
            return _mapper.Map<List<CountryDto>>(countries);
        }

        public async Task<CountryDto> GetCountryByIdAsync(int id)
        {
            var country = await _countryRepository.GetByIdAsync(id);
            return _mapper.Map<CountryDto>(country);
        }

        public async Task CreateCountryAsync(CountryDto countryDto)
        {
            var country = _mapper.Map<Country>(countryDto);
            await _countryRepository.CreateAsync(country);
        }

        public async Task UpdateCountryAsync(CountryDto countryDto)
        {
            var country = _mapper.Map<Country>(countryDto);
            await _countryRepository.UpdateAsync(country);
        }

        public async Task DeleteCountryAsync(int id)
        {
            var country = await _countryRepository.GetByIdAsync(id);
            if (country != null)
            {
                await _countryRepository.DeleteAsync(country);
            }
        }
    }
}