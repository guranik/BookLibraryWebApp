using BookLibraryBusinessLogicClassLibrary.DTOs.Genres;

namespace BookLibraryBusinessLogicClassLibrary.Interfaces
{
    public interface IGenreService
    {
        Task<List<GenreDto>> GetAllGenresAsync(CancellationToken cancellationToken);
        Task<GenreDto> GetGenreByIdAsync(int id, CancellationToken cancellationToken);
        Task CreateGenreAsync(GenreDto genreDto, CancellationToken cancellationToken);
        Task UpdateGenreAsync(GenreDto genreDto, CancellationToken cancellationToken);
        Task DeleteGenreAsync(int id, CancellationToken cancellationToken);
    }
}