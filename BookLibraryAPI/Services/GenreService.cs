using System.Collections.Generic;
using System.Linq;
using BookLibraryAPI.Models;
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

        public IEnumerable<Genre> AllGenres => _context.Genres;

        public Genre GetById(int id)
        {
            return _context.Genres.Include(g => g.Books).FirstOrDefault(g => g.Id == id) ?? 
                throw new InvalidOperationException($"Genre with ID {id} not found.");
        }

        public void Create(Genre genre)
        {
            _context.Genres.Add(genre);
            _context.SaveChanges();
        }

        public void Update(Genre genre)
        {
            _context.Genres.Update(genre);
            _context.SaveChanges();
        }

        public void Delete(Genre genre)
        {
            _context.Genres.Remove(genre);
            _context.SaveChanges();
        }
    }
}