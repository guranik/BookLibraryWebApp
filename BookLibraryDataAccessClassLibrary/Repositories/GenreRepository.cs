using System.Collections.Generic;
using System.Linq;
using System.Threading;
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

        public async Task<IEnumerable<Genre>> GetAllGenresAsync(CancellationToken cancellationToken)
            => await _context.Genres.ToListAsync(cancellationToken);

        public async Task<Genre> GetByIdAsync(int id, CancellationToken cancellationToken)
            => await _context.Genres.Include(g => g.Books).FirstOrDefaultAsync(g => g.Id == id, cancellationToken);

        public async Task CreateAsync(Genre genre, CancellationToken cancellationToken)
        {
            await _context.Genres.AddAsync(genre, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task UpdateAsync(Genre genre, CancellationToken cancellationToken)
        {
            var existingGenre = await GetByIdAsync(genre.Id, cancellationToken);
            existingGenre.Name = genre.Name;

            _context.Genres.Update(existingGenre);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task DeleteAsync(Genre genre, CancellationToken cancellationToken)
        {
            var existingGenre = await GetByIdAsync(genre.Id, cancellationToken);
            _context.Genres.Remove(existingGenre);
            await _context.SaveChangesAsync(cancellationToken);
        }

        private async Task<bool> GenreExistsAsync(Genre genre, CancellationToken cancellationToken)
            => await _context.Genres.AnyAsync(g => g.Name == genre.Name, cancellationToken);
    }
}