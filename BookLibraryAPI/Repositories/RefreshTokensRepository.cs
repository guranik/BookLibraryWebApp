using BookLibraryAPI.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace BookLibraryAPI.Repositories
{
    public class RefreshTokenRepository
    {
        private readonly Db15460Context _context;

        public RefreshTokenRepository(Db15460Context context)
        {
            _context = context;
        }

        public RefreshToken CreateRefreshTokenAsync(int userId)
        {
            var refreshToken = new RefreshToken
            {
                Token = GenerateRefreshToken(),
                UserId = userId,
                ExpiryDate = DateTime.UtcNow.AddDays(30)
            };

            _context.RefreshTokens.AddAsync(refreshToken);
            _context.SaveChangesAsync();

            return refreshToken;
        }

        public async Task<RefreshToken> GetValidRefreshTokenAsync(string token)
        {
            return await _context.RefreshTokens
                .FirstOrDefaultAsync(x => x.Token == token && !x.IsRevoked && x.ExpiryDate > DateTime.UtcNow) ??
                throw new InvalidOperationException($"Valid token {token} not found.");
        }

        public async Task RevokeRefreshTokenAsync(RefreshToken refreshToken)
        {
            refreshToken.IsRevoked = true;
            _context.RefreshTokens.Update(refreshToken);
            await _context.SaveChangesAsync();
        }

        private string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            RandomNumberGenerator.Fill(randomNumber);  // Новый метод генерации
            return Convert.ToBase64String(randomNumber);
        }
    }
}