using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BookLibraryAPI.Interfaces;
using BookLibraryAPI.Models;
using BookLibraryAPI.Services;

namespace BookLibraryAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GenreController : ControllerBase
    {
        private readonly IAllGenres _genreService;

        public GenreController(IAllGenres genreService)
        {
            _genreService = genreService;
        }

        [HttpGet]
        public IActionResult GetAllGenres()
        {
            var genres = _genreService.AllGenres;
            return Ok(genres);
        }

        [HttpGet("{id}")]
        public IActionResult GetGenreById(int id)
        {
            var genre = _genreService.GetById(id);
            return Ok(genre);
        }

        [Authorize(Policy = "AdminOnly")]
        [HttpPost]
        public IActionResult CreateGenre([FromBody] Genre genre)
        {
            _genreService.Create(genre);
            return CreatedAtAction(nameof(GetGenreById), new { id = genre.Id }, genre);
        }

        [Authorize(Policy = "AdminOnly")]
        [HttpPut("{id}")]
        public IActionResult UpdateGenre(int id, [FromBody] Genre genre)
        {
            if (id != genre.Id)
            {
                return BadRequest("ID из URL не соответствует ID жанра.");
            }

            _genreService.Update(genre);
            return NoContent();
        }

        [Authorize(Policy = "AdminOnly")]
        [HttpDelete("{id}")]
        public IActionResult DeleteGenre(int id)
        {
            var genre = _genreService.GetById(id);
            _genreService.Delete(genre);
            return NoContent();
        }
    }
}