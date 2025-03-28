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
using BookLibraryBusinessLogicClassLibrary.Interfaces;
using BookLibraryBusinessLogicClassLibrary.Exceptions;
using BookLibraryDataAccessClassLibrary.Repositories;

namespace BookLibraryBusinessLogicClassLibrary.Services
{
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

        public async Task<IdentityResult> RegisterUserAsync(RegisterModel registerUserDto, CancellationToken cancellationToken)
        {
            var user = _mapper.Map<User>(registerUserDto);
            var result = await _userManager.CreateAsync(user, registerUserDto.Password);

            if (!result.Succeeded)
            {
                throw new BadRequestException(string.Join(", ", result.Errors));
            }

            if (!string.IsNullOrEmpty(registerUserDto.Role))
            {
                await _userManager.AddToRoleAsync(user, registerUserDto.Role);
            }
            return result;
        }

        public async Task<(string Token, string RefreshToken)> LoginUserAsync(LoginModel loginModel, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByNameAsync(loginModel.Username);
            if (user == null || !await _userManager.CheckPasswordAsync(user, loginModel.Password))
            {
                throw new BadRequestException("Invalid username or password.");
            }

            var token = GenerateJwtToken(user);
            var refreshToken = await _refreshTokenRepository.CreateRefreshTokenAsync(user.Id, cancellationToken);
            return (token, refreshToken.Token);
        }

        public async Task<UserDto> GetUserAsync(int id, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null)
            {
                throw new NotFoundException($"User with ID {id} not found.");
            }
            return _mapper.Map<UserDto>(user);
        }

        public async Task<string> RefreshAccessTokenAsync(string refreshToken, CancellationToken cancellationToken)
        {
            var validRefreshToken = await _refreshTokenRepository.GetValidRefreshTokenAsync(refreshToken, cancellationToken);
            if (validRefreshToken == null)
            {
                throw new BadRequestException("Invalid refresh token.");
            }

            var user = await _userManager.FindByIdAsync(validRefreshToken.UserId.ToString());
            if (user == null)
            {
                throw new InvalidOperationException("User not found.");
            }

            return GenerateJwtToken(user);
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
    }
}