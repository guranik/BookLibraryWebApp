using BookLibraryClient.DTOs.Authors;
using BookLibraryClient.DTOs.IssuedBooks;
using BookLibraryClient.DTOs.Books;

namespace BookLibraryClient.DTOs.PagedResults
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
