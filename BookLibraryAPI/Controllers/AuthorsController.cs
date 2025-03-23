using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BookLibraryAPI.Interfaces;
using BookLibraryAPI.Models;
using AutoMapper;
using BookLibraryAPI.DTOs.Authors;
using BookLibraryAPI.DTOs.PagedResult;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BookLibraryAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthorsController : ControllerBase
    {
        private readonly IAllAuthors _authorService;
        private readonly IMapper _mapper;

        public AuthorsController(IAllAuthors authorService, IMapper mapper)
        {
            _authorService = authorService;
            _mapper = mapper;
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAllAuthors()
        {
            var authors = await _authorService.GetAllAuthorsAsync();
            var authorDtos = _mapper.Map<List<AuthorDto>>(authors);
            return Ok(authorDtos);
        }

        [HttpGet]
        public async Task<IActionResult> GetPagedAuthors(int pageNumber = 1, int pageSize = 10)
        {
            var pagedAuthors = await _authorService.GetPagedAuthorsAsync(pageNumber, pageSize);
            var authorDtos = _mapper.Map<List<AuthorDto>>(pagedAuthors.Items);
            var pagedAuthorsDto = new PagedAuthorsDto
            {
                Items = authorDtos,
                TotalPages = pagedAuthors.TotalPages,
                CurrentPage = pagedAuthors.PageNumber
            };
            return Ok(pagedAuthorsDto);
        }

        [HttpGet("sorted")]
        public async Task<IActionResult> GetSortedAuthors()
        {
            var authors = await _authorService.GetSortedAuthorsAsync();
            var authorDtos = _mapper.Map<List<AuthorDto>>(authors);
            return Ok(authorDtos);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetAuthorById(int id)
        {
            var author = await _authorService.GetByIdAsync(id);
            if (author == null)
            {
                return NotFound();
            }
            var authorDto = _mapper.Map<AuthorDto>(author);
            return Ok(authorDto);
        }

        [Authorize(Policy = "AdminOnly")]
        [HttpPost]
        public async Task<IActionResult> CreateAuthor([FromBody] AuthorDto authorDto)
        {
            var author = _mapper.Map<Author>(authorDto);
            await _authorService.CreateAsync(author);
            return CreatedAtAction(nameof(GetAuthorById), new { id = author.Id }, authorDto);
        }

        [Authorize(Policy = "AdminOnly")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAuthor(int id, [FromBody] AuthorDto authorDto)
        {
            if (id != authorDto.Id)
            {
                return BadRequest("ID из URL не соответствует ID автора.");
            }

            var author = _mapper.Map<Author>(authorDto);
            await _authorService.UpdateAsync(author);
            return NoContent();
        }

        [Authorize(Policy = "AdminOnly")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAuthor(int id)
        {
            var author = await _authorService.GetByIdAsync(id);
            if (author == null)
            {
                return NotFound();
            }
            await _authorService.DeleteAsync(author);
            return NoContent();
        }
    }
}