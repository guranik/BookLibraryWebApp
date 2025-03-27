using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookLibraryDataAccessClassLibrary.Models;
using BookLibraryDataAccessClassLibrary.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BookLibraryDataAccessClassLibrary.Repositories
{
    public class GenreRepository : IAllGenres
    {
        private readonly Db15460Context _context;

        public GenreRepository(Db15460Context context)
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
            if (!await GenreExistsAsync(genre))
            {
                await _context.Genres.AddAsync(genre);
                await _context.SaveChangesAsync();
            }
            else
            {
                throw new InvalidOperationException("Genre with this name already exists.");
            }
        }

        public async Task UpdateAsync(Genre genre)
        {
            var existingGenre = await GetByIdAsync(genre.Id);
            if (existingGenre != null)
            {
                existingGenre.Name = genre.Name;

                _context.Genres.Update(existingGenre);
                await _context.SaveChangesAsync();
            }
            else
            {
                throw new InvalidOperationException($"Genre with ID {genre.Id} not found.");
            }
        }

        public async Task DeleteAsync(Genre genre)
        {
            var existingGenre = await GetByIdAsync(genre.Id);
            if (existingGenre != null)
            {
                _context.Genres.Remove(existingGenre);
                await _context.SaveChangesAsync();
            }
            else
            {
                throw new InvalidOperationException($"Genre with ID {genre.Id} not found.");
            }
        }

        private async Task<bool> GenreExistsAsync(Genre genre)
        {
            return await _context.Genres.AnyAsync(g => g.Name == genre.Name);
        }
    }
}