using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using BookLibraryDataAccessClassLibrary.Interfaces;
using BookLibraryDataAccessClassLibrary.Models;
using BookLibraryBusinessLogicClassLibrary.DTOs.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using BookLibraryBusinessLogicClassLibrary.DTOs.Authentication;
using BookLibraryDataAccessClassLibrary.Repositories;

namespace BookLibraryBusinessLogicClassLibrary.Services
{
    public interface IUserService
    {
        Task<IdentityResult> RegisterUserAsync(RegisterModel registerUserDto);
        Task<(string Token, string RefreshToken)> LoginUserAsync(LoginModel loginModel);
        Task<UserDto> GetUserAsync(int id);
        Task<string> RefreshAccessTokenAsync(string refreshToken);
    }

    public class UserService : IUserService
    {
        private readonly UserManager<User> _userManager;
        private readonly IMapper _mapper;
        private readonly RefreshTokenRepository _refreshTokenRepository;
        private readonly IConfiguration _configuration;

        public UserService(UserManager<User> userManager, IMapper mapper, RefreshTokenRepository refreshTokenRepository, IConfiguration configuration)
        {
            _userManager = userManager;
            _mapper = mapper;
            _refreshTokenRepository = refreshTokenRepository;
            _configuration = configuration;
        }

        public async Task<IdentityResult> RegisterUserAsync(RegisterModel registerUserDto)
        {
            var user = _mapper.Map<User>(registerUserDto);
            var result = await _userManager.CreateAsync(user, registerUserDto.Password);

            if (result.Succeeded && !string.IsNullOrEmpty(registerUserDto.Role))
            {
                await _userManager.AddToRoleAsync(user, registerUserDto.Role);
            }

            return result;
        }

        public async Task<(string Token, string RefreshToken)> LoginUserAsync(LoginModel loginModel)
        {
            var user = await _userManager.FindByNameAsync(loginModel.Username);
            if (user != null && await _userManager.CheckPasswordAsync(user, loginModel.Password))
            {
                var token = GenerateJwtToken(user);
                var refreshToken = _refreshTokenRepository.CreateRefreshTokenAsync(user.Id);
                return (token, refreshToken.Token);
            }

            return (null, null); // Или выбросить исключение, если нужно
        }

        public async Task<UserDto> GetUserAsync(int id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            return _mapper.Map<UserDto>(user);
        }

        private string GenerateJwtToken(User user)
        {
            var roles = _userManager.GetRolesAsync(user).Result;

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, user.Id.ToString())
            };

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

        public async Task<string> RefreshAccessTokenAsync(string refreshToken)
        {
            // Получаем действующий refresh токен
            var validRefreshToken = await _refreshTokenRepository.GetValidRefreshTokenAsync(refreshToken);

            // Генерируем новый access токен
            var user = await _userManager.FindByIdAsync(validRefreshToken.UserId.ToString());
            if (user == null)
            {
                throw new InvalidOperationException("User not found.");
            }

            var newAccessToken = GenerateJwtToken(user);

            return newAccessToken;
        }
    }
}