using System.Collections.Generic;
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

        public async Task<IEnumerable<User>> GetAllUsersAsync()
            => await _context.Users.ToListAsync();

        public async Task<User> GetUserAsync(int id)
            => await _context.Users.FirstOrDefaultAsync(x => x.Id == id)
            ?? throw new InvalidOperationException($"User with ID {id} not found.");

        public async Task CreateAsync(User user)
        {
            if (await UserExistsAsync(user.Login))
            {
                throw new InvalidOperationException("User with this login already exists.");
            }

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(User user)
        {
            var existingUser = await GetUserAsync(user.Id);
            if (existingUser != null)
            {
                existingUser.Login = user.Login;
                existingUser.PasswordHash = user.PasswordHash; // Assuming you're updating the password hash as well.

                _context.Users.Update(existingUser);
                await _context.SaveChangesAsync();
            }
            else
            {
                throw new InvalidOperationException($"User with ID {user.Id} not found.");
            }
        }

        public async Task DeleteAsync(User user)
        {
            var existingUser = await GetUserAsync(user.Id);
            if (existingUser != null)
            {
                _context.Users.Remove(existingUser);
                await _context.SaveChangesAsync();
            }
            else
            {
                throw new InvalidOperationException($"User with ID {user.Id} not found.");
            }
        }

        private async Task<bool> UserExistsAsync(string login)
        {
            return await _context.Users.AnyAsync(u => u.Login == login);
        }
    }
}