using BookLibraryDataAccessClassLibrary.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace BookLibraryDataAccessClassLibrary.Repositories
{
    public class RefreshTokenRepository
    {
        private readonly Db15460Context _context;

        public RefreshTokenRepository(Db15460Context context)
        {
            _context = context;
        }

        public async Task<RefreshToken> CreateRefreshTokenAsync(int userId, CancellationToken cancellationToken)
        {
            var refreshToken = new RefreshToken
            {
                Token = GenerateRefreshToken(),
                UserId = userId,
                ExpiryDate = DateTime.UtcNow.AddDays(30)
            };

            await _context.RefreshTokens.AddAsync(refreshToken, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            return refreshToken;
        }

        public async Task<RefreshToken> GetValidRefreshTokenAsync(string token, CancellationToken cancellationToken)
        {
            return await _context.RefreshTokens
                .FirstOrDefaultAsync(x => x.Token == token && !x.IsRevoked && x.ExpiryDate > DateTime.UtcNow, cancellationToken) ??
                throw new InvalidOperationException($"Valid token {token} not found.");
        }

        public async Task RevokeRefreshTokenAsync(RefreshToken refreshToken, CancellationToken cancellationToken)
        {
            refreshToken.IsRevoked = true;
            _context.RefreshTokens.Update(refreshToken);
            await _context.SaveChangesAsync(cancellationToken);
        }

        private string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            RandomNumberGenerator.Fill(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }
    }
}