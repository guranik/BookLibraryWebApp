using System.Collections.Generic;
using System.Threading.Tasks;
using BookLibraryAPI.Interfaces;
using BookLibraryAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace BookLibraryAPI.Services
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
            await _context.Countries.AddAsync(country);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Country country)
        {
            _context.Countries.Update(country);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Country country)
        {
            _context.Countries.Remove(country);
            await _context.SaveChangesAsync();
        }
    }
}