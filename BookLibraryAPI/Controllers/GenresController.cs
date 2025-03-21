using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BookLibraryAPI.Interfaces;
using BookLibraryAPI.Models;
using BookLibraryAPI.DTOs.Genres;
using AutoMapper;
using System.Collections.Generic;
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
        public IActionResult GetAllGenres()
        {
            var genres = _genreService.AllGenres;
            var genreDtos = _mapper.Map<List<GenreDto>>(genres);
            return Ok(genreDtos);
        }

        [HttpGet("{id}")]
        public IActionResult GetGenreById(int id)
        {
            var genre = _genreService.GetById(id);
            if (genre == null)
            {
                return NotFound();
            }
            var genreDto = _mapper.Map<GenreDto>(genre);
            return Ok(genreDto);
        }

        [Authorize(Policy = "AdminOnly")]
        [HttpPost]
        public IActionResult CreateGenre([FromBody] GenreDto genreDto)
        {
            if (genreDto == null)
            {
                return BadRequest("Genre cannot be null.");
            }

            var genre = _mapper.Map<Genre>(genreDto);
            _genreService.Create(genre);
            return CreatedAtAction(nameof(GetGenreById), new { id = genre.Id }, genreDto);
        }

        [Authorize(Policy = "AdminOnly")]
        [HttpPut("{id}")]
        public IActionResult UpdateGenre(int id, [FromBody] GenreDto genreDto)
        {
            if (genreDto == null || genreDto.Id != id)
            {
                return BadRequest("Genre data is invalid.");
            }

            var genre = _mapper.Map<Genre>(genreDto);
            _genreService.Update(genre);
            return NoContent();
        }

        [Authorize(Policy = "AdminOnly")]
        [HttpDelete("{id}")]
        public IActionResult DeleteGenre(int id)
        {
            var genre = _genreService.GetById(id);
            if (genre == null)
            {
                return NotFound();
            }

            _genreService.Delete(genre);
            return NoContent();
        }
    }
}