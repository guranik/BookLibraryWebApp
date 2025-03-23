using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using BookLibraryClient.DTOs.IssuedBooks;
using BookLibraryClient.DTOs.PagedResults;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace BookLibraryClient.Controllers
{
    public class IssuedBooksController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public IssuedBooksController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        [Authorize(Roles = "User")]
        public async Task<IActionResult> Index(int pageNumber = 1, int pageSize = 10)
        {
            var client = await GetAuthorizedClientAsync();

            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "UserId");
            var userId = userIdClaim?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest("Пользователь не найден.");
            }

            var response = await client.GetAsync($"issuedbooks/user/{userId}?pageNumber={pageNumber}&pageSize={pageSize}");

            var jsonResponse = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
            {
                var pagedIssuedBooks = JsonConvert.DeserializeObject<PagedIssuedBooksDto>(jsonResponse);
                return View(pagedIssuedBooks);
            }
            else
            {
                return NotFound(new { Message = "No issued books found for this user." });
            }
        }

        [HttpPost("return/{issuedBookId}")]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> ReturnBook(int issuedBookId)
        {
            var client = await GetAuthorizedClientAsync();
            var response = await client.DeleteAsync($"books/return/{issuedBookId}");

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Index");
            }

            return NotFound();
        }

        private async Task<HttpClient> GetAuthorizedClientAsync()
        {
            var client = _httpClientFactory.CreateClient("BookLibraryAPI");
            var token = Request.Cookies["jwt"];

            if (!string.IsNullOrEmpty(token))
            {
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            }

            return client;
        }
    }
}