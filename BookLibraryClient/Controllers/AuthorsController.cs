using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using BookLibraryClient.DTOs.Authors;
using BookLibraryClient.DTOs.PagedResults;

namespace BookLibraryClient.Controllers
{
    public class AuthorsController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public AuthorsController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<IActionResult> Index()
        {
            var client = _httpClientFactory.CreateClient("BookLibraryAPI");
            var response = await client.GetAsync("authors");
            response.EnsureSuccessStatusCode();

            var jsonResponse = await response.Content.ReadAsStringAsync();
            var pagedAuthors = JsonConvert.DeserializeObject<PagedAuthorsDto>(jsonResponse);

            return View(pagedAuthors == null ? new List<AuthorDto>() : pagedAuthors.Items);
        }

        public async Task<IActionResult> Details(int id)
        {
            var client = _httpClientFactory.CreateClient("BookLibraryAPI");
            var response = await client.GetAsync($"authors/{id}");

            if (!response.IsSuccessStatusCode)
            {
                return NotFound();
            }

            var jsonResponse = await response.Content.ReadAsStringAsync();
            var authorDetails = JsonConvert.DeserializeObject<AuthorInfoDto>(jsonResponse);

            return View(authorDetails);
        }
    }
}