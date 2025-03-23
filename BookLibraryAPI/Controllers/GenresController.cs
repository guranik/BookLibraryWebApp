using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BookLibraryAPI.Interfaces;
using BookLibraryAPI.Models;
using BookLibraryAPI.DTOs.Genres;
using AutoMapper;
using System.Collections.Generic;
using System.Threading.Tasks;
using BookLibraryAPI.Services;

namespace BookLibraryAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GenresController : ControllerBase
    {
        private readonly IAllGenres _genreService;
        private readonly IMapper _mapper;

        public GenresController(IAllGenres genreService, IMapper mapper)
        {
            _genreService = genreService;
            _mapper = mapper;
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAllGenres()
        {
            var genres = await _genreService.GetAllGenresAsync();
            var genreDtos = _mapper.Map<List<GenreDto>>(genres);
            return Ok(genreDtos);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetGenreById(int id)
        {
            var genre = await _genreService.GetByIdAsync(id);
            if (genre == null)
            {
                return NotFound();
            }
            var genreDto = _mapper.Map<GenreDto>(genre);
            return Ok(genreDto);
        }

        [Authorize(Policy = "AdminOnly")]
        [HttpPost]
        public async Task<IActionResult> CreateGenre([FromBody] GenreDto genreDto)
        {
            if (genreDto == null)
            {
                return BadRequest("Genre cannot be null.");
            }

            var genre = _mapper.Map<Genre>(genreDto);
            await _genreService.CreateAsync(genre);
            return CreatedAtAction(nameof(GetGenreById), new { id = genre.Id }, genreDto);
        }

        [Authorize(Policy = "AdminOnly")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateGenre(int id, [FromBody] GenreDto genreDto)
        {
            if (genreDto == null || genreDto.Id != id)
            {
                return BadRequest("Genre data is invalid.");
            }

            var genre = _mapper.Map<Genre>(genreDto);
            await _genreService.UpdateAsync(genre);
            return NoContent();
        }

        [Authorize(Policy = "AdminOnly")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteGenre(int id)
        {
            var genre = await _genreService.GetByIdAsync(id);
            if (genre == null)
            {
                return NotFound();
            }

            await _genreService.DeleteAsync(genre);
            return NoContent();
        }
    }
}