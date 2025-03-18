using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BookLibraryAPI.Interfaces;
using BookLibraryAPI.Models;

namespace BookLibraryAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthorController : ControllerBase
    {
        private readonly IAllAuthors _authorService;

        public AuthorController(IAllAuthors authorService)
        {
            _authorService = authorService;
        }

        [HttpGet("all")]
        public IActionResult GetAllAuthors()
        {
            var authors = _authorService.AllAuthors;
            return Ok(authors);
        }

        [HttpGet("sorted")]
        public IActionResult GetSortedAuthors()
        {
            var authors = _authorService.SortedAuthors;
            return Ok(authors);
        }

        [HttpGet("{id}")]
        public IActionResult GetAuthorById(int id)
        {
            var author = _authorService.GetById(id);
            return Ok(author);
        }

        [Authorize(Policy = "AdminOnly")]
        [HttpPost]
        public IActionResult CreateAuthor([FromBody] Author author)
        {
            _authorService.Create(author);
            return CreatedAtAction(nameof(GetAuthorById), new { id = author.Id }, author);
        }

        [Authorize(Policy = "AdminOnly")]
        [HttpPut("{id}")]
        public IActionResult UpdateAuthor(int id, [FromBody] Author author)
        {
            if (id != author.Id)
            {
                return BadRequest("ID из URL не соответствует ID автора.");
            }

            _authorService.Update(author);
            return NoContent();
        }

        [Authorize(Policy = "AdminOnly")]
        [HttpDelete("{id}")]
        public IActionResult DeleteAuthor(int id)
        {
            var author = _authorService.GetById(id);
            _authorService.Delete(author);
            return NoContent();
        }
    }
}