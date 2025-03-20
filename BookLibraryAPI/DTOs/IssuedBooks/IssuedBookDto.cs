using BookLibraryAPI.DTOs.Books;

namespace BookLibraryAPI.DTOs.IssuedBooks
{
    public class IssuedBookDto
    {
        public int Id { get; set; }
        public int BookId { get; set; }
        public BookDto Book { get; set; } = null!;
        public DateTime Issued { get; set; }
        public DateTime Return { get; set; }
    }
}
