using Microsoft.AspNetCore.Mvc.Rendering;
using BookLibraryClient.DTOs.Authors;
using BookLibraryClient.DTOs.Genres;

namespace BookLibraryClient.ViewModels
{
    public class BookViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public string Description { get; set; } = null!;
        public int BookNumber { get; set; }
        public string? ISBN { get; set; }

        public int SelectedAuthorId { get; set; }
        public int SelectedGenreId { get; set; }

        public List<AuthorDto> Authors { get; set; } = new List<AuthorDto>();
        public List<GenreDto> Genres { get; set; } = new List<GenreDto>();
    }
}