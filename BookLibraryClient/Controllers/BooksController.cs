using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

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

            return View(pagedBooks == null ? new List<BookDto>() : pagedBooks.Items);
        }

        public IActionResult Details(int id)
        {
            // Logic to fetch and display book details
            return View();
        }
    }

    public class PagedBooksDto
    {
        public List<BookDto> Items { get; set; }
        public int TotalCount { get; set; }
    }

    public class BookDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public AuthorDto Author { get; set; }
        public string Genre { get; set; }
        public int BookNumber { get; set; }
    }

    public class AuthorDto
    {
        public string Name { get; set; }
        public string Surname { get; set; }
    }
}