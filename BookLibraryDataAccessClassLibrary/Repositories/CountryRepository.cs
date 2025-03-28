using System.Collections.Generic;
using System.Linq;
using System.Threading;
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

        public async Task<IEnumerable<Country>> GetAllCountriesAsync(CancellationToken cancellationToken)
            => await _context.Countries.ToListAsync(cancellationToken);

        public async Task<Country> GetByIdAsync(int id, CancellationToken cancellationToken)
            => await _context.Countries.FirstOrDefaultAsync(c => c.Id == id, cancellationToken);

        public async Task CreateAsync(Country country, CancellationToken cancellationToken)
        {
            await _context.Countries.AddAsync(country, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task UpdateAsync(Country country, CancellationToken cancellationToken)
        {
            var existingCountry = await GetByIdAsync(country.Id, cancellationToken);
            existingCountry.Name = country.Name;

            _context.Countries.Update(existingCountry);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task DeleteAsync(Country country, CancellationToken cancellationToken)
        {
            var existingCountry = await GetByIdAsync(country.Id, cancellationToken);
            _context.Countries.Remove(existingCountry);
            await _context.SaveChangesAsync(cancellationToken);
        }

        private async Task<bool> CountryExistsAsync(Country country, CancellationToken cancellationToken)
            => await _context.Countries.AnyAsync(c => c.Name == country.Name, cancellationToken);
    }
}