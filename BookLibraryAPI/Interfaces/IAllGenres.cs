using BookLibraryAPI.Models;
using System.Collections.Generic;

namespace BookLibraryAPI.Services
{
    public interface IAllGenres
    {
        IEnumerable<Genre> AllGenres { get; }
        Genre GetById(int id);
        void Create(Genre genre);
        void Update(Genre genre);
        void Delete(Genre genre);
    }
}