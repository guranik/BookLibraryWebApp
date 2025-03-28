using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using BookLibraryBusinessLogicClassLibrary.DTOs.Users;
using BookLibraryBusinessLogicClassLibrary.DTOs.Authentication;
using BookLibraryBusinessLogicClassLibrary.Interfaces;

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
        public async Task<IActionResult> Register([FromBody] RegisterModel registerUserDto, CancellationToken cancellationToken)
        {
            await _userService.RegisterUserAsync(registerUserDto, cancellationToken);
            return Ok(new { Message = "User registered successfully." });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel loginModel, CancellationToken cancellationToken)
        {
            var (token, refreshToken) = await _userService.LoginUserAsync(loginModel, cancellationToken);
            return Ok(new { Token = token, RefreshToken = refreshToken });
        }

        [Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUser(int id, CancellationToken cancellationToken)
        {
            var userDto = await _userService.GetUserAsync(id, cancellationToken);
            return Ok(userDto);
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenModel refreshTokenModel, CancellationToken cancellationToken)
        {
            var newAccessToken = await _userService.RefreshAccessTokenAsync(refreshTokenModel.RefreshToken, cancellationToken);
            return Ok(new { Token = newAccessToken });
        }
    }
}