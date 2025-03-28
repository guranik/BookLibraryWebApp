using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using BookLibraryDataAccessClassLibrary.Interfaces;
using BookLibraryDataAccessClassLibrary.Models;
using Microsoft.EntityFrameworkCore;

namespace BookLibraryDataAccessClassLibrary.Repositories
{
    public class AuthorRepository : IAllAuthors
    {
        private readonly Db15460Context _context;

        public AuthorRepository(Db15460Context context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Author>> GetAllAuthorsAsync(CancellationToken cancellationToken)
            => await _context.Authors.ToListAsync(cancellationToken);

        public async Task<Author> GetByIdAsync(int id, CancellationToken cancellationToken)
            => await _context.Authors.Include(a => a.Books).FirstOrDefaultAsync(a => a.Id == id, cancellationToken);

        public async Task CreateAsync(Author author, CancellationToken cancellationToken)
        {
            await _context.Authors.AddAsync(author, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task UpdateAsync(Author author, CancellationToken cancellationToken)
        {
            var existingAuthor = await GetByIdAsync(author.Id, cancellationToken);
            existingAuthor.Name = author.Name;
            existingAuthor.Surname = author.Surname;
            existingAuthor.BirthDate = author.BirthDate;
            existingAuthor.CountryId = author.CountryId;

            _context.Authors.Update(existingAuthor);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task DeleteAsync(Author author, CancellationToken cancellationToken)
        {
            var existingAuthor = await GetByIdAsync(author.Id, cancellationToken);
            _context.Authors.Remove(existingAuthor);
            await _context.SaveChangesAsync(cancellationToken);
        }

        private async Task<bool> AuthorExistsAsync(Author author, CancellationToken cancellationToken)
            => await _context.Authors.AnyAsync(a =>
                a.Id == author.Id ||
                (a.Name == author.Name &&
                 a.Surname == author.Surname &&
                 a.BirthDate == author.BirthDate &&
                 a.CountryId == author.CountryId), cancellationToken);
    }
}