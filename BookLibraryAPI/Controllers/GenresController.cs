﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BookLibraryDataAccessClassLibrary.Interfaces;
using BookLibraryBusinessLogicClassLibrary.DTOs.Genres;
using System.Collections.Generic;
using System.Threading.Tasks;
using BookLibraryBusinessLogicClassLibrary.Services;

namespace BookLibraryAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GenresController : ControllerBase
    {
        private readonly IGenreService _genreService;

        public GenresController(IGenreService genreService)
        {
            _genreService = genreService;
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAllGenres()
        {
            var genreDtos = await _genreService.GetAllGenresAsync();
            return Ok(genreDtos);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetGenreById(int id)
        {
            var genreDto = await _genreService.GetGenreByIdAsync(id);
            if (genreDto == null)
            {
                return NotFound();
            }
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

            await _genreService.CreateGenreAsync(genreDto);
            return CreatedAtAction(nameof(GetGenreById), new { id = genreDto.Id }, genreDto);
        }

        [Authorize(Policy = "AdminOnly")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateGenre(int id, [FromBody] GenreDto genreDto)
        {
            if (genreDto == null || genreDto.Id != id)
            {
                return BadRequest("Genre data is invalid.");
            }

            await _genreService.UpdateGenreAsync(genreDto);
            return NoContent();
        }

        [Authorize(Policy = "AdminOnly")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteGenre(int id)
        {
            var genre = await _genreService.GetGenreByIdAsync(id);
            if (genre == null)
            {
                return NotFound();
            }

            await _genreService.DeleteGenreAsync(id);
            return NoContent();
        }
    }
}