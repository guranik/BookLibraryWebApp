using System.Collections.Generic;
using System.Threading.Tasks;
using BookLibraryDataAccessClassLibrary.Models;
using BookLibraryDataAccessClassLibrary.ViewModels;
using System.Threading;

namespace BookLibraryDataAccessClassLibrary.Interfaces
{
    public interface IAllBooks
    {
        Task<IEnumerable<Book>> GetAllBooksAsync(CancellationToken cancellationToken);
        Task<PagedList<Book>> GetPagedBooksAsync(string genre, string author, string bookName, int pageNumber, int pageSize, CancellationToken cancellationToken);
        Task<Book> GetByIdAsync(int id, CancellationToken cancellationToken);
        Task<Book> GetByISBNAsync(string isbn, CancellationToken cancellationToken);
        Task IssueBookAsync(int bookId, CancellationToken cancellationToken);
        Task ReturnBookAsync(int bookId, CancellationToken cancellationToken);
        Task<IEnumerable<Book>> GetByAuthorAsync(int authorId, CancellationToken cancellationToken);
        Task UpdateAsync(Book book, CancellationToken cancellationToken);
        Task DeleteAsync(Book book, CancellationToken cancellationToken);
        Task CreateAsync(Book book, CancellationToken cancellationToken);
        Task<bool> AreAllBooksIssuedAsync(string title, string authorName, CancellationToken cancellationToken);
    }
}