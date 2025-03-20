using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using BookLibraryClient.DTOs.PagedResults;
using BookLibraryClient.DTOs.Books;

namespace BookLibraryClient.Controllers
{
    public class BooksController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public BooksController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<IActionResult> Index()
        {
            var client = _httpClientFactory.CreateClient("BookLibraryAPI");
            var response = await client.GetAsync("books/search");
            response.EnsureSuccessStatusCode();

            var jsonResponse = await response.Content.ReadAsStringAsync();
            var pagedBooks = JsonConvert.DeserializeObject<PagedBooksDto>(jsonResponse);

            return View(books);
        }

        public async Task<IActionResult> Details(int id)
        {
            var client = _httpClientFactory.CreateClient("BookLibraryAPI");
            var response = await client.GetAsync($"books/{id}");

            if (!response.IsSuccessStatusCode)
            {
                return NotFound();
            }

            var jsonResponse = await response.Content.ReadAsStringAsync();
            var bookDetails = JsonConvert.DeserializeObject<BookInfoDto>(jsonResponse);

            return View(bookDetails);
        }
    }
}