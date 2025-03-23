using System.Globalization;

namespace BookLibraryAPI.Models
{
    public class Book
    {
        public int Id { get; set; }
        public string? ISBN { get; set; }
        public string Title { get; set; } = null!;
        public string Description { get; set; } = null!;
        public int AuthorId { get; set; }
        public Author? Author { get; set; }
        public int GenreId { get; set; }
        public Genre? Genre { get; set; }
        public int BookNumber { get; set; }
        public string? ImageUrl { get; set; }
        public ICollection<IssuedBook> IssuedBooks { get; set; } = new List<IssuedBook>();
    }
}
