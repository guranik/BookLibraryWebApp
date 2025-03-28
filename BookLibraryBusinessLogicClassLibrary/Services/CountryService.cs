﻿using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using BookLibraryDataAccessClassLibrary.Interfaces;
using BookLibraryDataAccessClassLibrary.Models;
using BookLibraryBusinessLogicClassLibrary.DTOs.Countries;
using BookLibraryBusinessLogicClassLibrary.Exceptions;
using BookLibraryBusinessLogicClassLibrary.Interfaces;

namespace BookLibraryBusinessLogicClassLibrary.Services
{
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
            if (country == null)
            {
                throw new NotFoundException($"Country with ID {id} not found.");
            }
            return _mapper.Map<CountryDto>(country);
        }

        public async Task CreateCountryAsync(CountryDto countryDto, CancellationToken cancellationToken)
        {
            if (countryDto == null)
            {
                throw new BadRequestException("Country cannot be null.");
            }

            var country = _mapper.Map<Country>(countryDto);
            await _countryRepository.CreateAsync(country, cancellationToken);
        }

        public async Task UpdateCountryAsync(CountryDto countryDto, CancellationToken cancellationToken)
        {
            if (countryDto == null || countryDto.Id <= 0)
            {
                throw new BadRequestException("Country data is invalid.");
            }

            var existingCountry = await _countryRepository.GetByIdAsync(countryDto.Id, cancellationToken);
            if (existingCountry == null)
            {
                throw new NotFoundException($"Country with ID {countryDto.Id} not found.");
            }

            await _countryRepository.UpdateAsync(existingCountry, cancellationToken);
        }

        public async Task DeleteCountryAsync(int id, CancellationToken cancellationToken)
        {
            var country = await _countryRepository.GetByIdAsync(id, cancellationToken);
            if (country == null)
            {
                throw new NotFoundException($"Country with ID {id} not found.");
            }

            await _countryRepository.DeleteAsync(country, cancellationToken);
        }
    }
}