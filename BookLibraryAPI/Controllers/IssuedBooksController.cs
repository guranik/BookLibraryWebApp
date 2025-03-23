using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BookLibraryAPI.Interfaces;
using BookLibraryAPI.DTOs.IssuedBooks;
using System.Threading.Tasks;
using BookLibraryAPI.Services;

namespace BookLibraryAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class IssuedBooksController : ControllerBase
    {
        private readonly IIssuedBookService _issuedBookService;

        public IssuedBooksController(IIssuedBookService issuedBookService)
        {
            _issuedBookService = issuedBookService;
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetByUser(int userId, int pageNumber = 1, int pageSize = 10)
        {
            var issuedBooksDto = await _issuedBookService.GetByUserAsync(userId, pageNumber, pageSize);
            if (issuedBooksDto.Items.Count == 0)
            {
                return NotFound(new { Message = "No issued books found for this user." });
            }
            return Ok(issuedBooksDto);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var issuedBookDto = await _issuedBookService.GetByIdAsync(id);
            if (issuedBookDto == null)
            {
                return NotFound();
            }
            return Ok(issuedBookDto);
        }

        [HttpPost]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> Create([FromBody] IssuedBookDto issuedBookDto)
        {
            if (issuedBookDto == null)
            {
                return BadRequest("Issued Book cannot be null.");
            }
            await _issuedBookService.CreateAsync(issuedBookDto);
            return CreatedAtAction(nameof(GetById), new { id = issuedBookDto.Id }, issuedBookDto);
        }

        [HttpPut("{id}")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> Update(int id, [FromBody] IssuedBookDto issuedBookDto)
        {
            if (issuedBookDto == null || issuedBookDto.Id != id)
            {
                return BadRequest("Issued Book data is invalid.");
            }
            await _issuedBookService.UpdateAsync(id, issuedBookDto);
            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> Delete(int id)
        {
            var issuedBook = await _issuedBookService.GetByIdAsync(id);
            if (issuedBook == null)
            {
                return NotFound();
            }
            await _issuedBookService.DeleteAsync(id);
            return NoContent();
        }
    }
}