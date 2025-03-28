﻿using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BookLibraryDataAccessClassLibrary.Interfaces;
using BookLibraryDataAccessClassLibrary.Models;
using BookLibraryDataAccessClassLibrary.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace BookLibraryDataAccessClassLibrary.Repositories
{
    public class IssuedBookRepository : IAllIssuedBooks
    {
        private readonly Db15460Context _context;

        public IssuedBookRepository(Db15460Context context)
        {
            _context = context;
        }

        public async Task<PagedList<IssuedBook>> GetPagedIssuedBooksAsync(int page, int pageSize, CancellationToken cancellationToken)
        {
            IQueryable<IssuedBook> issuedBooks = _context.IssuedBooks.Include(b => b.Book).Include(b => b.User);

            var totalCount = await issuedBooks.CountAsync(cancellationToken);
            var items = await issuedBooks.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(cancellationToken);

            return new PagedList<IssuedBook>(items, totalCount, page, pageSize);
        }

        public async Task<PagedList<IssuedBook>> GetByUserAsync(int userId, int pageNumber, int pageSize, CancellationToken cancellationToken)
        {
            var query = _context.IssuedBooks
                .Include(ib => ib.Book)
                    .ThenInclude(b => b.Author)
                .Include(ib => ib.Book)
                    .ThenInclude(b => b.Genre)
                .Include(ib => ib.User)
                .Where(ib => ib.UserId == userId);

            var totalCount = await query.CountAsync(cancellationToken);
            var items = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync(cancellationToken);

            return new PagedList<IssuedBook>(items, totalCount, pageNumber, pageSize);
        }

        public async Task<IssuedBook> GetByIdAsync(int id, CancellationToken cancellationToken)
        {
            return await _context.IssuedBooks.Include(ib => ib.Book).Include(ib => ib.User)
                .FirstOrDefaultAsync(ib => ib.Id == id, cancellationToken);
        }

        public async Task CreateAsync(IssuedBook issuedBook, CancellationToken cancellationToken)
        {
            await _context.IssuedBooks.AddAsync(issuedBook, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task UpdateAsync(IssuedBook issuedBook, CancellationToken cancellationToken)
        {
            var existingIssuedBook = await GetByIdAsync(issuedBook.Id, cancellationToken);
            existingIssuedBook.BookId = issuedBook.BookId;
            existingIssuedBook.UserId = issuedBook.UserId;
            existingIssuedBook.Issued = issuedBook.Issued;
            existingIssuedBook.Return = issuedBook.Return;

            _context.IssuedBooks.Update(existingIssuedBook);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task DeleteAsync(IssuedBook issuedBook, CancellationToken cancellationToken)
        {
            var existingIssuedBook = await GetByIdAsync(issuedBook.Id, cancellationToken);
            _context.IssuedBooks.Remove(existingIssuedBook);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}