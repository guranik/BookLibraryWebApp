using BookLibraryBusinessLogicClassLibrary.DTOs.Users;
using Microsoft.AspNetCore.Identity;
using BookLibraryBusinessLogicClassLibrary.DTOs.Authentication;

namespace BookLibraryBusinessLogicClassLibrary.Interfaces
{
    public interface IUserService
    {
        Task<IdentityResult> RegisterUserAsync(RegisterModel registerUserDto, CancellationToken cancellationToken);
        Task<(string Token, string RefreshToken)> LoginUserAsync(LoginModel loginModel, CancellationToken cancellationToken);
        Task<UserDto> GetUserAsync(int id, CancellationToken cancellationToken);
        Task<string> RefreshAccessTokenAsync(string refreshToken, CancellationToken cancellationToken);
    }
}