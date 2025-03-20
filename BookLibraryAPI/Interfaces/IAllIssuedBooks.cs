using BookLibraryAPI.Models;
using BookLibraryAPI.ViewModels;

namespace BookLibraryAPI.Interfaces
{
    public interface IAllIssuedBooks
    {
        public PagedList<IssuedBook> GetPagedIssuedBooks(int page, int pageSize);
        public PagedList<IssuedBook> GetByUser(int userId, int pageNumber, int pageSize);
        public IssuedBook GetById(int id);
        public void Create(IssuedBook issuedBook);
        public void Update(IssuedBook issuedBook);
        public void Delete(IssuedBook issuedBook);
    }
}
