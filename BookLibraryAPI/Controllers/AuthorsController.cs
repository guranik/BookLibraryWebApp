using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BookLibraryBusinessLogicClassLibrary.DTOs.Authors;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using BookLibraryBusinessLogicClassLibrary.Interfaces;

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
        public async Task<IActionResult> GetAllAuthors(CancellationToken cancellationToken)
        {
            var authorDtos = await _authorService.GetAllAuthorsAsync(cancellationToken);
            return Ok(authorDtos);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetAuthorById(int id, CancellationToken cancellationToken)
        {
            var authorDto = await _authorService.GetAuthorByIdAsync(id, cancellationToken);
            return Ok(authorDto);
        }

        [Authorize(Policy = "AdminOnly")]
        [HttpPost]
        public async Task<IActionResult> CreateAuthor([FromBody] AuthorInfoDto authorDto, CancellationToken cancellationToken)
        {
            await _authorService.CreateAuthorAsync(authorDto, cancellationToken);
            return CreatedAtAction(nameof(GetAuthorById), new { id = authorDto.Id }, authorDto);
        }

        [Authorize(Policy = "AdminOnly")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAuthor(int id, [FromBody] AuthorInfoDto authorDto, CancellationToken cancellationToken)
        {
            authorDto.Id = id;
            await _authorService.UpdateAuthorAsync(authorDto, cancellationToken);
            return NoContent();
        }

        [Authorize(Policy = "AdminOnly")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAuthor(int id, CancellationToken cancellationToken)
        {
            await _authorService.DeleteAuthorAsync(id, cancellationToken);
            return NoContent();
        }
    }
}