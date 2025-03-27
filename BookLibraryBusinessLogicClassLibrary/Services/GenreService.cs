using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using BookLibraryDataAccessClassLibrary.Interfaces;
using BookLibraryDataAccessClassLibrary.Models;
using BookLibraryBusinessLogicClassLibrary.DTOs.Genres;

namespace BookLibraryBusinessLogicClassLibrary.Services
{
    public interface IGenreService
    {
        Task<List<GenreDto>> GetAllGenresAsync(CancellationToken cancellationToken);
        Task<GenreDto> GetGenreByIdAsync(int id, CancellationToken cancellationToken);
        Task CreateGenreAsync(GenreDto genreDto, CancellationToken cancellationToken);
        Task UpdateGenreAsync(GenreDto genreDto, CancellationToken cancellationToken);
        Task DeleteGenreAsync(int id, CancellationToken cancellationToken);
    }

    public class GenreService : IGenreService
    {
        private readonly IAllGenres _genreRepository;
        private readonly IMapper _mapper;

        public GenreService(IAllGenres genreRepository, IMapper mapper)
        {
            _genreRepository = genreRepository;
            _mapper = mapper;
        }

        public async Task<List<GenreDto>> GetAllGenresAsync(CancellationToken cancellationToken)
        {
            var genres = await _genreRepository.GetAllGenresAsync(cancellationToken);
            return _mapper.Map<List<GenreDto>>(genres);
        }

        public async Task<GenreDto> GetGenreByIdAsync(int id, CancellationToken cancellationToken)
        {
            var genre = await _genreRepository.GetByIdAsync(id, cancellationToken);
            return _mapper.Map<GenreDto>(genre);
        }

        public async Task CreateGenreAsync(GenreDto genreDto, CancellationToken cancellationToken)
        {
            var genre = _mapper.Map<Genre>(genreDto);
            await _genreRepository.CreateAsync(genre, cancellationToken);
        }

        public async Task UpdateGenreAsync(GenreDto genreDto, CancellationToken cancellationToken)
        {
            var genre = _mapper.Map<Genre>(genreDto);
            await _genreRepository.UpdateAsync(genre, cancellationToken);
        }

        public async Task DeleteGenreAsync(int id, CancellationToken cancellationToken)
        {
            var genre = await _genreRepository.GetByIdAsync(id, cancellationToken);
            if (genre != null)
            {
                await _genreRepository.DeleteAsync(genre, cancellationToken);
            }
        }
    }
}