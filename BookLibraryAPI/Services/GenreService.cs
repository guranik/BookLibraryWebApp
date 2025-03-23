using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using BookLibraryAPI.Interfaces;
using BookLibraryAPI.Models;
using BookLibraryAPI.DTOs.Genres;

namespace BookLibraryAPI.Services
{
    public interface IGenreService
    {
        Task<List<GenreDto>> GetAllGenresAsync();
        Task<GenreDto> GetGenreByIdAsync(int id);
        Task CreateGenreAsync(GenreDto genreDto);
        Task UpdateGenreAsync(GenreDto genreDto);
        Task DeleteGenreAsync(int id);
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

        public async Task<List<GenreDto>> GetAllGenresAsync()
        {
            var genres = await _genreRepository.GetAllGenresAsync();
            return _mapper.Map<List<GenreDto>>(genres);
        }

        public async Task<GenreDto> GetGenreByIdAsync(int id)
        {
            var genre = await _genreRepository.GetByIdAsync(id);
            return _mapper.Map<GenreDto>(genre);
        }

        public async Task CreateGenreAsync(GenreDto genreDto)
        {
            var genre = _mapper.Map<Genre>(genreDto);
            await _genreRepository.CreateAsync(genre);
        }

        public async Task UpdateGenreAsync(GenreDto genreDto)
        {
            var genre = _mapper.Map<Genre>(genreDto);
            await _genreRepository.UpdateAsync(genre);
        }

        public async Task DeleteGenreAsync(int id)
        {
            var genre = await _genreRepository.GetByIdAsync(id);
            if (genre != null)
            {
                await _genreRepository.DeleteAsync(genre);
            }
        }
    }
}