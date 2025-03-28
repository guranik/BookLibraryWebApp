using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BookLibraryBusinessLogicClassLibrary.DTOs.Genres;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using BookLibraryBusinessLogicClassLibrary.Interfaces;

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
        public async Task<IActionResult> GetAllGenres(CancellationToken cancellationToken)
        {
            var genreDtos = await _genreService.GetAllGenresAsync(cancellationToken);
            return Ok(genreDtos);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetGenreById(int id, CancellationToken cancellationToken)
        {
            var genreDto = await _genreService.GetGenreByIdAsync(id, cancellationToken);
            return Ok(genreDto);
        }

        [Authorize(Policy = "AdminOnly")]
        [HttpPost]
        public async Task<IActionResult> CreateGenre([FromBody] GenreDto genreDto, CancellationToken cancellationToken)
        {
            await _genreService.CreateGenreAsync(genreDto, cancellationToken);
            return CreatedAtAction(nameof(GetGenreById), new { id = genreDto.Id }, genreDto);
        }

        [Authorize(Policy = "AdminOnly")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateGenre(int id, [FromBody] GenreDto genreDto, CancellationToken cancellationToken)
        {
            genreDto.Id = id;
            await _genreService.UpdateGenreAsync(genreDto, cancellationToken);
            return NoContent();
        }

        [Authorize(Policy = "AdminOnly")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteGenre(int id, CancellationToken cancellationToken)
        {
            await _genreService.DeleteGenreAsync(id, cancellationToken);
            return NoContent();
        }
    }
}