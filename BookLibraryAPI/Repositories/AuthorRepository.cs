using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookLibraryAPI.Interfaces;
using BookLibraryAPI.Models;
using BookLibraryAPI.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace BookLibraryAPI.Services
{
    public class AuthorRepository : IAllAuthors
    {
        private readonly Db15460Context _context;

        public AuthorRepository(Db15460Context context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Author>> GetAllAuthorsAsync()
            => await _context.Authors.ToListAsync();

        public async Task<IEnumerable<Author>> GetSortedAuthorsAsync()
            => await _context.Authors
                .Include(a => a.Books)
                .OrderBy(a => a.Surname)
                .ThenBy(a => a.Name)
                .ThenBy(a => a.BirthDate)
                .ToListAsync();

        public async Task<PagedList<Author>> GetPagedAuthorsAsync(int page, int pageSize)
        {
            IQueryable<Author> authors = _context.Authors.Include(a => a.Country);

            var totalCount = await authors.CountAsync();
            var items = await authors.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

            return new PagedList<Author>(items, totalCount, page, pageSize);
        }

        public async Task<Author> GetByIdAsync(int id)
        {
            return await _context.Authors.Include(a => a.Books).FirstOrDefaultAsync(a => a.Id == id)
                ?? throw new InvalidOperationException($"Автор с ID {id} не найден.");
        }

        public async Task CreateAsync(Author author)
        {
            await _context.Authors.AddAsync(author);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Author author)
        {
            _context.Authors.Update(author);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Author author)
        {
            _context.Authors.Remove(author);
            await _context.SaveChangesAsync();
        }
    }
}