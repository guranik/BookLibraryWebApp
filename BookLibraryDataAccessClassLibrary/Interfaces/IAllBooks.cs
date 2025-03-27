using System.Collections.Generic;
using System.Threading.Tasks;
using BookLibraryDataAccessClassLibrary.Models;
using BookLibraryDataAccessClassLibrary.ViewModels;

namespace BookLibraryDataAccessClassLibrary.Interfaces
{
    public interface IAllBooks
    {
        Task<IEnumerable<Book>> GetAllBooksAsync();
        Task<PagedList<Book>> GetPagedBooksAsync(string genre, string author, string bookName, int pageNumber, int pageSize);
        Task<Book> GetByIdAsync(int id);
        Task<Book> GetByISBNAsync(string isbn);
        Task IssueBookAsync(int bookId);
        Task ReturnBookAsync(int bookId);
        Task<IEnumerable<Book>> GetByAuthorAsync(int authorId);
        Task UpdateAsync(Book book);
        Task DeleteAsync(Book book);
        Task CreateAsync(Book book);
        Task<bool> AreAllBooksIssuedAsync(string title, string authorName);
    }
}