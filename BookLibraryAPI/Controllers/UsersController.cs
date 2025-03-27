using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using BookLibraryAPI.DTOs.Users;
using BookLibraryAPI.Interfaces;
using BookLibraryAPI.Services;

namespace BookLibraryAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IConfiguration _configuration;

        public UserController(IUserService userService, IConfiguration configuration)
        {
            _userService = userService;
            _configuration = configuration;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterModel registerUserDto)
        {
            var result = await _userService.RegisterUserAsync(registerUserDto);
            if (result.Succeeded)
            {
                return Ok(new { Message = "User registered successfully." });
            }

            return BadRequest(result.Errors);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel loginModel)
        {
            var (token, refreshToken) = await _userService.LoginUserAsync(loginModel);
            if (token != null)
            {
                return Ok(new { Token = token, RefreshToken = refreshToken });
            }

            return Unauthorized();
        }

        [Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUser(int id)
        {
            var userDto = await _userService.GetUserAsync(id);
            if (userDto == null)
            {
                return NotFound();
            }
            return Ok(userDto);
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenModel refreshTokenModel)
        {
            var newAccessToken = await _userService.RefreshAccessTokenAsync(refreshTokenModel.RefreshToken);
            return Ok(new { Token = newAccessToken });
        }
    }

    public class RefreshTokenModel
    {
        public required string RefreshToken { get; set; }
    }

    public class LoginModel
    {
        public required string Username { get; set; }
        public required string Password { get; set; }
    }

    public class RegisterModel
    {
        public required string Login { get; set; }
        public required string Password { get; set; }
        public required string Role { get; set; }
    }
}