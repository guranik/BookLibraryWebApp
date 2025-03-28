using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BookLibraryBusinessLogicClassLibrary.DTOs.IssuedBooks;
using System.Threading;
using System.Threading.Tasks;
using BookLibraryBusinessLogicClassLibrary.Interfaces;

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

        [Authorize]
        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetByUser(int userId, int pageNumber = 1, int pageSize = 10, CancellationToken cancellationToken = default)
        {
            var issuedBooksDto = await _issuedBookService.GetByUserAsync(userId, pageNumber, pageSize, cancellationToken);
            return Ok(issuedBooksDto);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken = default)
        {
            var issuedBookDto = await _issuedBookService.GetByIdAsync(id, cancellationToken);
            return Ok(issuedBookDto);
        }

        [HttpPost]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> Create([FromBody] IssuedBookDto issuedBookDto, CancellationToken cancellationToken = default)
        {
            await _issuedBookService.CreateAsync(issuedBookDto, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = issuedBookDto.Id }, issuedBookDto);
        }

        [HttpPut("{id}")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> Update(int id, [FromBody] IssuedBookDto issuedBookDto, CancellationToken cancellationToken = default)
        {
            await _issuedBookService.UpdateAsync(id, issuedBookDto, cancellationToken);
            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken = default)
        {
            await _issuedBookService.DeleteAsync(id, cancellationToken);
            return NoContent();
        }
    }
}