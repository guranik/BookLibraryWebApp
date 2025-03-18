using System.Globalization;

namespace BookLibraryAPI.Models
{
    public class Book
    {
        public int Id { get; set; }
        public string ISBN { get; set; } = null!;
        public string Title { get; set; } = null!;
        public string Description { get; set; } = null!;
        public int AuthorId { get; set; }
        public Author Author { get; set; } = null!;
        public int GenreId { get; set; }
        public Genre Genre { get; set; } = null!;
        public int BookNumber { get; set; }
        public ICollection<IssuedBook> IssuedBooks { get; set; } = new List<IssuedBook>();
    }
}
