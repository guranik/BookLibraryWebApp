using System.Collections.Generic;
using System.Linq;
using BookLibraryAPI.Interfaces;
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

        public PagedList<IssuedBook> GetPagedIssuedBooks(int page, int pageSize)
        {
            IQueryable<IssuedBook> issuedBooks = _context.IssuedBooks.Include(b => b.Book).Include(b => b.User);

            var totalCount = issuedBooks.Count();
            var items = issuedBooks.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            return new PagedList<IssuedBook>(items, totalCount, page, pageSize);
        }

        public PagedList<IssuedBook> GetByUser(int userId, int pageNumber, int pageSize)
        {
            var query = _context.IssuedBooks.Include(ib => ib.Book).Include(ib => ib.User)
                .Where(ib => ib.UserId == userId);

            var totalCount = query.Count();
            var items = query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

            return new PagedList<IssuedBook>(items, totalCount, pageNumber, pageSize);
        }

        public IssuedBook GetById(int id)
        {
            return _context.IssuedBooks.Include(ib => ib.Book).Include(ib => ib.User)
                .FirstOrDefault(ib => ib.Id == id) ??
                throw new InvalidOperationException($"Issued book with ID {id} not found.");
        }

        public void Create(IssuedBook issuedBook)
        {
            _context.IssuedBooks.Add(issuedBook);
            _context.SaveChanges();
        }

        public void Update(IssuedBook issuedBook)
        {
            _context.IssuedBooks.Update(issuedBook);
            _context.SaveChanges();
        }

        public void Delete(IssuedBook issuedBook)
        {
            _context.IssuedBooks.Remove(issuedBook);
            _context.SaveChanges();
        }
    }
}