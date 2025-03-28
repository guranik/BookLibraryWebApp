using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using BookLibraryDataAccessClassLibrary.Interfaces;
using BookLibraryDataAccessClassLibrary.Models;
using Microsoft.EntityFrameworkCore;

namespace BookLibraryDataAccessClassLibrary.Repositories
{
    public class UserRepository : IAllUsers
    {
        private readonly Db15460Context _context;

        public UserRepository(Db15460Context context)
        {
            _context = context;
        }

        public async Task<IEnumerable<User>> GetAllUsersAsync(CancellationToken cancellationToken)
            => await _context.Users.ToListAsync(cancellationToken);

        public async Task<User> GetUserAsync(int id, CancellationToken cancellationToken)
            => await _context.Users.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        public async Task CreateAsync(User user, CancellationToken cancellationToken)
        {
            await _context.Users.AddAsync(user, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task UpdateAsync(User user, CancellationToken cancellationToken)
        {
            var existingUser = await GetUserAsync(user.Id, cancellationToken);
            existingUser.Login = user.Login;
            existingUser.PasswordHash = user.PasswordHash; // Update password hash as well.

            _context.Users.Update(existingUser);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task DeleteAsync(User user, CancellationToken cancellationToken)
        {
            var existingUser = await GetUserAsync(user.Id, cancellationToken);
            _context.Users.Remove(existingUser);
            await _context.SaveChangesAsync(cancellationToken);
        }

        private async Task<bool> UserExistsAsync(string login, CancellationToken cancellationToken)
            => await _context.Users.AnyAsync(u => u.Login == login, cancellationToken);
    }
}