using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookLibraryAPI.Interfaces;
using BookLibraryAPI.Middleware;
using BookLibraryAPI.Models;
using BookLibraryAPI.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace BookLibraryAPI.Services
{
    public class IssuedBookService : IAllIssuedBooks
    {
        private readonly Db15460Context _context;

        public IssuedBookService(Db15460Context context)
        {
            _context = context;
        }

        public async Task<PagedList<IssuedBook>> GetPagedIssuedBooksAsync(int page, int pageSize)
        {
            IQueryable<IssuedBook> issuedBooks = _context.IssuedBooks.Include(b => b.Book).Include(b => b.User);

            var totalCount = await issuedBooks.CountAsync();
            var items = await issuedBooks.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

            return new PagedList<IssuedBook>(items, totalCount, page, pageSize);
        }

        public async Task<PagedList<IssuedBook>> GetByUserAsync(int userId, int pageNumber, int pageSize)
        {
            var query = _context.IssuedBooks
                .Include(ib => ib.Book)
                    .ThenInclude(b => b.Author)
                .Include(ib => ib.Book)
                    .ThenInclude(b => b.Genre)
                .Include(ib => ib.User)
                .Where(ib => ib.UserId == userId);

            var totalCount = await query.CountAsync();
            var items = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();

            return new PagedList<IssuedBook>(items, totalCount, pageNumber, pageSize);
        }

        public async Task<IssuedBook> GetByIdAsync(int id)
        {
            return await _context.IssuedBooks.Include(ib => ib.Book).Include(ib => ib.User)
                .FirstOrDefaultAsync(ib => ib.Id == id)
                ?? throw new InvalidOperationException($"Issued book with ID {id} not found.");
        }

        public async Task CreateAsync(IssuedBook issuedBook)
        {
            if (await _context.IssuedBooks.AnyAsync(b => b.UserId == issuedBook.UserId && b.BookId == issuedBook.BookId))
            {
                throw new BookIsAlreadyIssuedException("Вы уже взяли данную книгу.");
            }

            await _context.IssuedBooks.AddAsync(issuedBook);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(IssuedBook issuedBook)
        {
            _context.IssuedBooks.Update(issuedBook);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(IssuedBook issuedBook)
        {
            _context.IssuedBooks.Remove(issuedBook);
            await _context.SaveChangesAsync();
        }
    }
}