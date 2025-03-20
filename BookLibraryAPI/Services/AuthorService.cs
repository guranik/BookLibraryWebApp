using System.Collections.Generic;
using System.Linq;
using BookLibraryAPI.Interfaces;
using BookLibraryAPI.Models;
using BookLibraryAPI.ViewModels;
using Microsoft.EntityFrameworkCore;
using static System.Reflection.Metadata.BlobBuilder;

namespace BookLibraryAPI.Services
{
    public class AuthorService : IAllAuthors
    {
        private readonly Db15460Context _context;

        public AuthorService(Db15460Context context)
        {
            _context = context;
        }

        public IEnumerable<Author> AllAuthors => _context.Authors;

        public IEnumerable<Author> SortedAuthors => _context.Authors
            .Include(a => a.Books)
            .OrderBy(a => a.Surname)
            .ThenBy(a => a.Name)
            .ThenBy(a => a.BirthDate);

        public PagedList<Author> GetPagedAuthors(int page, int pageSize)
        {
            IQueryable<Author> authors = _context.Authors.Include(a => a.Country);

            var totalCount = authors.Count();
            var items = authors.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            return new PagedList<Author>(items, totalCount, page, pageSize);
        }

        public Author GetById(int id)
        {
            return _context.Authors.Include(a => a.Books).FirstOrDefault(a => a.Id == id) ??
                throw new InvalidOperationException($"Автор с ID {id} не найден.");
        }

        public void Create(Author author)
        {
            _context.Authors.Add(author);
            _context.SaveChanges();
        }

        public void Update(Author author)
        {
            _context.Authors.Update(author);
            _context.SaveChanges();
        }

        public void Delete(Author author)
        {
            _context.Authors.Remove(author);
            _context.SaveChanges();
        }
    }
}