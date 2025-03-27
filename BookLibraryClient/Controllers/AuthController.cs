using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.IdentityModel.Tokens.Jwt;

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
                var jsonResponse = await response.Content.ReadAsStringAsync();
                var tokenResponse = JsonSerializer.Deserialize<TokenResponse>(jsonResponse);

                if (tokenResponse != null && !string.IsNullOrEmpty(tokenResponse.token))
                {
                    Response.Cookies.Append("jwt", tokenResponse.token, new CookieOptions
                    {
                        HttpOnly = true,
                        Secure = true,
                        SameSite = SameSiteMode.Strict
                    });
                    Response.Cookies.Append("refreshToken", tokenResponse.refreshToken, new CookieOptions
                    {
                        HttpOnly = true,
                        Secure = true,
                        SameSite = SameSiteMode.Strict
                    });

                    var handler = new JwtSecurityTokenHandler();
                    var jwtToken = handler.ReadJwtToken(tokenResponse.token);

                    var username = jwtToken.Claims.First(claim => claim.Type == "sub").Value;
                    var userId = jwtToken.Claims.First(claim => claim.Type == "jti").Value;
                    var role = jwtToken.Claims.First(claim => claim.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role").Value; 

                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, username),
                        new Claim("UserId", userId),
                        new Claim(ClaimTypes.Role, role)
                    };

                    var claimsIdentity = new ClaimsIdentity(claims, "Login");
                    var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
                    await HttpContext.SignInAsync(claimsPrincipal);

                    return RedirectToAction("Index", "Books");
                }
            }

            ModelState.AddModelError("", "Login failed.");
            return View("Index");
        }

        public class TokenResponse
        {
            public string token { get; set; }
            public string refreshToken { get; set; }
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