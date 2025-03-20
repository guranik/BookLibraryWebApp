using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BookLibraryAPI.Interfaces;
using BookLibraryAPI.Models;
using AutoMapper;
using BookLibraryAPI.DTOs.Authors;
using BookLibraryAPI.DTOs.PagedResult;
using System.Collections.Generic;

namespace BookLibraryAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthorController : ControllerBase
    {
        private readonly IAllAuthors _authorService;
        private readonly IMapper _mapper;

        public AuthorController(IAllAuthors authorService, IMapper mapper)
        {
            _authorService = authorService;
            _mapper = mapper;
        }

        [HttpGet("all")]
        public IActionResult GetAllAuthors()
        {
            var authors = _authorService.AllAuthors;
            var authorDtos = _mapper.Map<List<AuthorDto>>(authors);
            return Ok(authorDtos);
        }

        [HttpGet]
        public IActionResult GetPagedAuthors(int pageNumber = 1, int pageSize = 10)
        {
            var pagedAuthors = _authorService.GetPagedAuthors(pageNumber, pageSize);
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
        public IActionResult GetSortedAuthors()
        {
            var authors = _authorService.SortedAuthors;
            var authorDtos = _mapper.Map<List<AuthorDto>>(authors);
            return Ok(authorDtos);
        }

        [HttpGet("{id}")]
        public IActionResult GetAuthorById(int id)
        {
            var author = _authorService.GetById(id);
            if (author == null)
            {
                return NotFound();
            }
            var authorDto = _mapper.Map<AuthorDto>(author);
            return Ok(authorDto);
        }

        [Authorize(Policy = "AdminOnly")]
        [HttpPost]
        public IActionResult CreateAuthor([FromBody] AuthorDto authorDto)
        {
            var author = _mapper.Map<Author>(authorDto);
            _authorService.Create(author);
            return CreatedAtAction(nameof(GetAuthorById), new { id = author.Id }, authorDto);
        }

        [Authorize(Policy = "AdminOnly")]
        [HttpPut("{id}")]
        public IActionResult UpdateAuthor(int id, [FromBody] AuthorDto authorDto)
        {
            if (id != authorDto.Id)
            {
                return BadRequest("ID из URL не соответствует ID автора.");
            }

            var author = _mapper.Map<Author>(authorDto);
            _authorService.Update(author);
            return NoContent();
        }

        [Authorize(Policy = "AdminOnly")]
        [HttpDelete("{id}")]
        public IActionResult DeleteAuthor(int id)
        {
            var author = _authorService.GetById(id);
            if (author == null)
            {
                return NotFound();
            }
            _authorService.Delete(author);
            return NoContent();
        }
    }
}