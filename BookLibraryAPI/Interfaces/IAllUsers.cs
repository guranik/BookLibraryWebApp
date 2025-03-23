using System.Collections.Generic;
using System.Threading.Tasks;
using BookLibraryAPI.Models;

namespace BookLibraryAPI.Interfaces
{
    public interface IAllUsers
    {
        Task<IEnumerable<User>> GetAllUsersAsync();
        Task<User> GetUserAsync(int id);
        Task CreateAsync(User user);
        Task UpdateAsync(User user);
        Task DeleteAsync(User user);
    }
}