using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookLibraryDataAccessClassLibrary.Interfaces;
using BookLibraryDataAccessClassLibrary.Models;
using Microsoft.EntityFrameworkCore;

namespace BookLibraryDataAccessClassLibrary.Repositories
{
    public class CountryRepository : IAllCountries
    {
        private readonly Db15460Context _context;

        public CountryRepository(Db15460Context context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Country>> GetAllCountriesAsync()
            => await _context.Countries.ToListAsync();

        public async Task<Country> GetByIdAsync(int id)
        {
            return await _context.Countries.FirstOrDefaultAsync(c => c.Id == id)
                ?? throw new InvalidOperationException($"Страна с ID {id} не найдена.");
        }

        public async Task CreateAsync(Country country)
        {
            if (!await CountryExistsAsync(country))
            {
                await _context.Countries.AddAsync(country);
                await _context.SaveChangesAsync();
            }
            else
            {
                throw new InvalidOperationException("Страна с таким названием уже существует.");
            }
        }

        public async Task UpdateAsync(Country country)
        {
            var existingCountry = await GetByIdAsync(country.Id);
            if (existingCountry != null)
            {
                existingCountry.Name = country.Name;

                _context.Countries.Update(existingCountry);
                await _context.SaveChangesAsync();
            }
            else
            {
                throw new InvalidOperationException($"Страна с ID {country.Id} не найдена.");
            }
        }

        public async Task DeleteAsync(Country country)
        {
            var existingCountry = await GetByIdAsync(country.Id);
            if (existingCountry != null)
            {
                _context.Countries.Remove(existingCountry);
                await _context.SaveChangesAsync();
            }
            else
            {
                throw new InvalidOperationException($"Страна с ID {country.Id} не найдена.");
            }
        }

        private async Task<bool> CountryExistsAsync(Country country)
        {
            return await _context.Countries.AnyAsync(c => c.Name == country.Name);
        }
    }
}