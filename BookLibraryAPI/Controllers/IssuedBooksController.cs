using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BookLibraryAPI.Models;
using BookLibraryAPI.Interfaces;
using System.Threading.Tasks;
using System.Linq;

namespace BookLibraryAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class IssuedBooksController : ControllerBase
    {
        private readonly IAllIssuedBooks _issuedBookService;

        public IssuedBooksController(IAllIssuedBooks issuedBookService)
        {
            _issuedBookService = issuedBookService;
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetByUser(int userId, int pageNumber = 1, int pageSize = 10)
        {
            var issuedBooks = await Task.Run(() => _issuedBookService.GetByUser(userId, pageNumber, pageSize));

            if (!issuedBooks.Items.Any())
            {
                return NotFound(new { Message = "No issued books found for this user." });
            }

            return Ok(issuedBooks);
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var issuedBook = _issuedBookService.GetById(id);
            return Ok(issuedBook);
        }

        [HttpPost]
        [Authorize(Policy = "AdminOnly")]
        public IActionResult Create([FromBody] IssuedBook issuedBook)
        {
            _issuedBookService.Create(issuedBook);
            return CreatedAtAction(nameof(GetById), new { id = issuedBook.Id }, issuedBook);
        }

        [HttpPut("{id}")]
        [Authorize(Policy = "AdminOnly")]
        public IActionResult Update(int id, [FromBody] IssuedBook issuedBook)
        {
            if (id != issuedBook.Id)
            {
                return BadRequest("Issued Book ID mismatch.");
            }

            _issuedBookService.Update(issuedBook);
            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Policy = "AdminOnly")]
        public IActionResult Delete(int id)
        {
            var issuedBook = _issuedBookService.GetById(id);
            _issuedBookService.Delete(issuedBook);
            return NoContent();
        }
    }
}