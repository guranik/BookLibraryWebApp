using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using BookLibraryClient.DTOs.Books;
using BookLibraryClient.ViewModels;
using BookLibraryClient.DTOs.Authors;
using BookLibraryClient.DTOs.Genres;
using BookLibraryClient.DTOs.PagedResults;
using Microsoft.AspNetCore.Authorization;

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
            var client = await GetAuthorizedClientAsync();
            var response = await client.GetAsync("books/search");
            response.EnsureSuccessStatusCode();

            var jsonResponse = await response.Content.ReadAsStringAsync();
            var pagedBooks = JsonConvert.DeserializeObject<PagedBooksDto>(jsonResponse);
            return View(pagedBooks == null? new List<BookDto>() : pagedBooks.Items);
        }

        public async Task<IActionResult> Create()
        {
            var model = new BookViewModel
            {
                Authors = await GetAuthorsAsync(),
                Genres = await GetGenresAsync()
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(BookViewModel model)
        {
            if (ModelState.IsValid)
            {
                var bookDto = new BookInfoDto
                {
                    Title = model.Title,
                    Description = model.Description,
                    BookNumber = model.BookNumber,
                    ISBN = model.ISBN,
                    AuthorId = model.SelectedAuthorId,
                    GenreId = model.SelectedGenreId
                };

                var client = await GetAuthorizedClientAsync();
                var response = await client.PostAsJsonAsync("books", bookDto);
                response.EnsureSuccessStatusCode();

                return RedirectToAction("Index");
            }

            model.Authors = await GetAuthorsAsync();
            model.Genres = await GetGenresAsync();
            return View(model);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var client = await GetAuthorizedClientAsync();
            var response = await client.GetAsync($"books/{id}");
            if (!response.IsSuccessStatusCode) return NotFound();

            var jsonResponse = await response.Content.ReadAsStringAsync();
            var bookInfo = JsonConvert.DeserializeObject<BookInfoDto>(jsonResponse);

            var model = new BookViewModel
            {
                Id = bookInfo.Id,
                Title = bookInfo.Title,
                Description = bookInfo.Description,
                BookNumber = bookInfo.BookNumber,
                ISBN = bookInfo.ISBN,
                SelectedAuthorId = bookInfo.Author.Id,
                SelectedGenreId = bookInfo.Genre.Id,
                Authors = await GetAuthorsAsync(),
                Genres = await GetGenresAsync()
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(BookViewModel model)
        {
            if (ModelState.IsValid)
            {
                var bookDto = new BookInfoDto
                {
                    Id = model.Id,
                    Title = model.Title,
                    Description = model.Description,
                    BookNumber = model.BookNumber,
                    ISBN = model.ISBN,
                    AuthorId = model.SelectedAuthorId,
                    GenreId = model.SelectedGenreId
                };

                var client = await GetAuthorizedClientAsync();
                var response = await client.PutAsJsonAsync($"books/{model.Id}", bookDto);
                response.EnsureSuccessStatusCode();

                return RedirectToAction("Index");
            }

            model.Authors = await GetAuthorsAsync();
            model.Genres = await GetGenresAsync();
            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var client = await GetAuthorizedClientAsync();
            var response = await client.DeleteAsync($"books/{id}");

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

        private async Task<List<AuthorDto>> GetAuthorsAsync()
        {
            var client = _httpClientFactory.CreateClient("BookLibraryAPI");
            var response = await client.GetAsync("authors/all");
            response.EnsureSuccessStatusCode();

            var jsonResponse = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<List<AuthorDto>>(jsonResponse);
        }

        private async Task<List<GenreDto>> GetGenresAsync()
        {
            var client = _httpClientFactory.CreateClient("BookLibraryAPI");
            var response = await client.GetAsync("genres/all");
            response.EnsureSuccessStatusCode();

            var jsonResponse = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<List<GenreDto>>(jsonResponse);
        }
    }
}