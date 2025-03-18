using BookLibraryAPI.Models;
using BookLibraryAPI.ViewModels;

namespace BookLibraryAPI.Interfaces
{
    public interface IAllBooks
    {
        public IEnumerable<Book> AllBooks { get; }
        public PagedList<Book> GetPagedBooks(string genre, string author, string bookName, int pageNumber, int pageSize);
        public Book GetById(int id);
        public Book GetByISBN(string name);
        public IEnumerable<Book> GetByAuthor(int authorId);
        public void Update(Book book);
        public void Delete(Book book);
        public void Create(Book book);
        public bool AreAllBooksIssued(string title, int authorId);
    }
}
