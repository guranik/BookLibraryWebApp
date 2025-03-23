using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookLibraryAPI.Models;
using BookLibraryAPI.Services;
using Microsoft.EntityFrameworkCore;

namespace BookLibraryAPI.Services
{
    public class GenreService : IAllGenres
    {
        private readonly Db15460Context _context;

        public GenreService(Db15460Context context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Genre>> GetAllGenresAsync()
            => await _context.Genres.ToListAsync();

        public async Task<Genre> GetByIdAsync(int id)
        {
            return await _context.Genres.Include(g => g.Books).FirstOrDefaultAsync(g => g.Id == id)
                ?? throw new InvalidOperationException($"Genre with ID {id} not found.");
        }

        public async Task CreateAsync(Genre genre)
        {
            await _context.Genres.AddAsync(genre);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Genre genre)
        {
            _context.Genres.Update(genre);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Genre genre)
        {
            _context.Genres.Remove(genre);
            await _context.SaveChangesAsync();
        }
    }
}