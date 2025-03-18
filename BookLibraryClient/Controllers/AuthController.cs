using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace BookLibraryClient.Controllers
{
    public class AuthController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public AuthController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterUserDto registerUserDto)
        {
            var client = _httpClientFactory.CreateClient("BookLibraryAPI");
            var content = new StringContent(JsonSerializer.Serialize(registerUserDto), Encoding.UTF8, "application/json");
            var response = await client.PostAsync("user/register", content);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Index","Books");
            }

            ModelState.AddModelError("", "Registration failed.");
            return View("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginModel loginModel)
        {
            var client = _httpClientFactory.CreateClient("BookLibraryAPI");
            var content = new StringContent(JsonSerializer.Serialize(loginModel), Encoding.UTF8, "application/json");
            var response = await client.PostAsync("user/login", content);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Index", "Books");
            }

            ModelState.AddModelError("", "Login failed.");
            return View("Index");
        }
    }

    public class RegisterUserDto
    {
        public string Login { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }
    }

    public class LoginModel
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}