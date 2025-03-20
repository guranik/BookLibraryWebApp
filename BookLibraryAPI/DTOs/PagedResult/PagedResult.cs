using BookLibraryAPI.DTOs.Authors;
using BookLibraryAPI.DTOs.IssuedBooks;
using BookLibraryAPI.DTOs.Books;

namespace BookLibraryAPI.DTOs.PagedResult
{
    public class PagedResult<T>
    {
        public List<T> Items { get; set; } = new List<T>();
        public int TotalPages { get; set; }
        public int CurrentPage { get; set; }
    }

    public class PagedAuthorsDto : PagedResult<AuthorDto> { }

    public class PagedIssuedBooksDto : PagedResult<IssuedBookDto> { }

    public class PagedBooksDto : PagedResult<BookDto> { }
}
