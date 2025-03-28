using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using BookLibraryDataAccessClassLibrary.Interfaces;
using BookLibraryDataAccessClassLibrary.Models;
using BookLibraryBusinessLogicClassLibrary.DTOs.Genres;
using BookLibraryBusinessLogicClassLibrary.Interfaces;
using BookLibraryBusinessLogicClassLibrary.Exceptions;

namespace BookLibraryBusinessLogicClassLibrary.Services
{
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
            if (genre == null)
            {
                throw new NotFoundException($"Genre with ID {id} not found.");
            }
            return _mapper.Map<GenreDto>(genre);
        }

        public async Task CreateGenreAsync(GenreDto genreDto, CancellationToken cancellationToken)
        {
            if (genreDto == null)
            {
                throw new BadRequestException("Genre cannot be null.");
            }

            var genre = _mapper.Map<Genre>(genreDto);
            await _genreRepository.CreateAsync(genre, cancellationToken);
        }

        public async Task UpdateGenreAsync(GenreDto genreDto, CancellationToken cancellationToken)
        {
            if (genreDto == null || genreDto.Id <= 0)
            {
                throw new BadRequestException("Genre data is invalid.");
            }

            var genre = await _genreRepository.GetByIdAsync(genreDto.Id, cancellationToken);
            if (genre == null)
            {
                throw new NotFoundException($"Genre with ID {genreDto.Id} not found.");
            }

            genre = _mapper.Map<Genre>(genreDto);
            await _genreRepository.UpdateAsync(genre, cancellationToken);
        }

        public async Task DeleteGenreAsync(int id, CancellationToken cancellationToken)
        {
            var genre = await _genreRepository.GetByIdAsync(id, cancellationToken);
            if (genre == null)
            {
                throw new NotFoundException($"Genre with ID {id} not found.");
            }

            await _genreRepository.DeleteAsync(genre, cancellationToken);
        }
    }
}