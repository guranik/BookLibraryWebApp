using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BookLibraryAPI.Interfaces;
using BookLibraryAPI.DTOs.Authors;
using BookLibraryAPI.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BookLibraryAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthorsController : ControllerBase
    {
        private readonly IAuthorService _authorService;

        public AuthorsController(IAuthorService authorService)
        {
            _authorService = authorService;
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAllAuthors()
        {
            var authorDtos = await _authorService.GetAllAuthorsAsync();
            return Ok(authorDtos);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetAuthorById(int id)
        {
            var authorDto = await _authorService.GetAuthorByIdAsync(id);
            if (authorDto == null)
            {
                return NotFound();
            }
            return Ok(authorDto);
        }

        [Authorize(Policy = "AdminOnly")]
        [HttpPost]
        public async Task<IActionResult> CreateAuthor([FromBody] AuthorDto authorDto)
        {
            await _authorService.CreateAuthorAsync(authorDto);
            return CreatedAtAction(nameof(GetAuthorById), new { id = authorDto.Id }, authorDto);
        }

        [Authorize(Policy = "AdminOnly")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAuthor(int id, [FromBody] AuthorDto authorDto)
        {
            if (id != authorDto.Id)
            {
                return BadRequest("ID из URL не соответствует ID автора.");
            }

            await _authorService.UpdateAuthorAsync(authorDto);
            return NoContent();
        }

        [Authorize(Policy = "AdminOnly")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAuthor(int id)
        {
            await _authorService.DeleteAuthorAsync(id);
            return NoContent();
        }
    }
}