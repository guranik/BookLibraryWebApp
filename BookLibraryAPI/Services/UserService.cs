using System.Collections.Generic;
using System.Threading.Tasks;
using BookLibraryAPI.Interfaces;
using BookLibraryAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace BookLibraryAPI.Services
{
    public class UserService : IAllUsers
    {
        private readonly Db15460Context _context;

        public UserService(Db15460Context context)
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
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(User user)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(User user)
        {
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
        }
    }
}