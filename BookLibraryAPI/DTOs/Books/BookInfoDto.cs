using BookLibraryAPI.DTOs.Authors;
using BookLibraryAPI.DTOs.Genres;

namespace BookLibraryAPI.DTOs.Books
{
    public class BookInfoDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public string Description { get; set; } = null!;
        public AuthorDto Author { get; set; } = null!;
        public GenreDto Genre { get; set; } = null!;
        public int BookNumber { get; set; }
        public string? ISBN { get; set; }
    }
}
