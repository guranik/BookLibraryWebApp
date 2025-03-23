using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BookLibraryAPI.Interfaces;
using BookLibraryAPI.Models;
using BookLibraryAPI.DTOs.IssuedBooks;
using BookLibraryAPI.DTOs.PagedResult;
using AutoMapper;
using System.Linq;
using System.Threading.Tasks;

namespace BookLibraryAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class IssuedBooksController : ControllerBase
    {
        private readonly IAllIssuedBooks _issuedBookService;
        private readonly IMapper _mapper;

        public IssuedBooksController(IAllIssuedBooks issuedBookService, IMapper mapper)
        {
            _issuedBookService = issuedBookService;
            _mapper = mapper;
        }

        [HttpGet("user/{userId}")]
        public IActionResult GetByUser(int userId, int pageNumber = 1, int pageSize = 10)
        {
            var issuedBooks = _issuedBookService.GetByUser(userId, pageNumber, pageSize);

            if (!issuedBooks.Items.Any())
            {
                return NotFound(new { Message = "No issued books found for this user." });
            }

            var issuedBookDtos = _mapper.Map<List<IssuedBookDto>>(issuedBooks.Items);
            return Ok(new PagedIssuedBooksDto
            {
                Items = issuedBookDtos,
                TotalPages = issuedBooks.TotalPages,
                CurrentPage = issuedBooks.PageNumber
            });
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var issuedBook = _issuedBookService.GetById(id);
            if (issuedBook == null)
            {
                return NotFound();
            }

            var issuedBookDto = _mapper.Map<IssuedBookDto>(issuedBook);
            return Ok(issuedBookDto);
        }

        [HttpPost]
        [Authorize(Policy = "AdminOnly")]
        public IActionResult Create([FromBody] IssuedBookDto issuedBookDto)
        {
            if (issuedBookDto == null)
            {
                return BadRequest("Issued Book cannot be null.");
            }

            var issuedBook = _mapper.Map<IssuedBook>(issuedBookDto);
            _issuedBookService.Create(issuedBook);
            return CreatedAtAction(nameof(GetById), new { id = issuedBook.Id }, issuedBookDto);
        }

        [HttpPut("{id}")]
        [Authorize(Policy = "AdminOnly")]
        public IActionResult Update(int id, [FromBody] IssuedBookDto issuedBookDto)
        {
            if (issuedBookDto == null || issuedBookDto.Id != id)
            {
                return BadRequest("Issued Book data is invalid.");
            }

            var issuedBook = _mapper.Map<IssuedBook>(issuedBookDto);
            _issuedBookService.Update(issuedBook);
            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Policy = "AdminOnly")]
        public IActionResult Delete(int id)
        {
            var issuedBook = _issuedBookService.GetById(id);
            if (issuedBook == null)
            {
                return NotFound();
            }

            _issuedBookService.Delete(issuedBook);
            return NoContent();
        }
    }
}