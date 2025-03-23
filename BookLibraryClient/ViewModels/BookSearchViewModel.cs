using BookLibraryClient.DTOs.Authors;
using BookLibraryClient.DTOs.Genres;
using BookLibraryClient.DTOs.Books;

namespace BookLibraryClient.ViewModels
{
    public class BookSearchViewModel
    {
        public List<BookDto> Books { get; set; } = new List<BookDto>();
        public List<AuthorDto> Authors { get; set; } = new List<AuthorDto>();
        public List<GenreDto> Genres { get; set; } = new List<GenreDto>();
        public string? SelectedGenre { get; set; }
        public string? SelectedAuthor { get; set; }
        public string? BookName { get; set; }
        public string? ErrorMessage { get; set; }
    }
}
