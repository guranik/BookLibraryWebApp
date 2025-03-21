using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using BookLibraryAPI.Models;
using BookLibraryAPI.Services;
using AutoMapper;
using Microsoft.AspNetCore.Razor.TagHelpers;
using BookLibraryAPI.Interfaces;
using BookLibraryAPI.DTOs.Users;

namespace BookLibraryAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly IConfiguration _configuration;
        private readonly RefreshTokenService _refreshTokenService;
        private readonly IMapper _mapper;
        private readonly IAllUsers _userService;

        public UserController(UserManager<User> userManager, IConfiguration configuration, RefreshTokenService refreshTokenService, IMapper mapper, IAllUsers userService)
        {
            _userManager = userManager;
            _configuration = configuration;
            _refreshTokenService = refreshTokenService;
            _mapper = mapper;
            _userService = userService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterModel registerUserDto)
        {
            var user = _mapper.Map<User>(registerUserDto);
            var result = await _userManager.CreateAsync(user, registerUserDto.Password);

            if (result.Succeeded)
            {
                if (!string.IsNullOrEmpty(registerUserDto.Role))
                {
                    var roleResult = await _userManager.AddToRoleAsync(user, registerUserDto.Role);
                    if (!roleResult.Succeeded)
                    {
                        return BadRequest(roleResult.Errors);
                    }
                }

                return Ok(new { Message = "User registered successfully." });
            }

            return BadRequest(result.Errors);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel loginModel)
        {
            var user = await _userManager.FindByNameAsync(loginModel.Username);

            if (user != null && await _userManager.CheckPasswordAsync(user, loginModel.Password))
            {
                var token = GenerateJwtToken(user);
                var refreshToken = _refreshTokenService.CreateRefreshTokenAsync(user.Id);

                return Ok(new { Token = token, RefreshToken = refreshToken.Token });
            }

            return Unauthorized();
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh([FromBody] RefreshTokenModel model)
        {
            var storedToken = await _refreshTokenService.GetValidRefreshTokenAsync(model.RefreshToken);
            if (storedToken == null)
            {
                return Unauthorized();
            }

            var user = await _userManager.FindByIdAsync(storedToken.UserId.ToString());
            if (user == null)
            {
                return Unauthorized();
            }

            var newToken = GenerateJwtToken(user);

            await _refreshTokenService.RevokeRefreshTokenAsync(storedToken);
            var newRefreshToken = _refreshTokenService.CreateRefreshTokenAsync(user.Id);

            return Ok(new { Token = newToken, RefreshToken = newRefreshToken.Token });
        }

        private async Task<string> GenerateJwtToken(User user)
        {
            if (user.UserName == null)
                throw new ArgumentNullException(nameof(user.UserName));

            // Получение ролей пользователя
            var roles = await _userManager.GetRolesAsync(user);

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, user.Id.ToString())
            };

            // Добавление ролей как утверждений
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"] ?? throw new ArgumentNullException("JWT Key cannot be null")));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Issuer"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        [Authorize]
        [HttpGet("{id}")]
        public IActionResult GetUser(int id)
        {
            var user = _userService.GetUser(id); // Обновите, если используете AutoMapper здесь
            var userDto = _mapper.Map<UserDto>(user);
            return Ok(userDto);
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