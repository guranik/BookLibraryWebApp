namespace BookLibraryAPI.Dtos.Books
{
    public class BookDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public AuthorDto Author { get; set; } = null!;
        public string Genre { get; set; } = null!;
        public int BookNumber { get; set; }
    }

    public class AuthorDto
    {
        public string Name { get; set; } = null!;
        public string Surname { get; set; } = null!;
    }

    public class PagedBooksDto
    {
        public List<BookDto> Items { get; set; } = new List<BookDto>();
        public int TotalCount { get; set; }
    }
}