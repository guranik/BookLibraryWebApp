using BookLibraryAPI.Models;
using BookLibraryAPI.ViewModels;

namespace BookLibraryAPI.Interfaces
{
    public interface IAllAuthors
    {
        public IEnumerable<Author> AllAuthors { get; }
        public IEnumerable<Author> SortedAuthors { get; }
        public PagedList<Author> GetPagedAuthors(int page, int pageSize); 
        public Author GetById(int id);
        public void Update(Author author);
        public void Delete(Author author);
        public void Create(Author author);
    }
}
