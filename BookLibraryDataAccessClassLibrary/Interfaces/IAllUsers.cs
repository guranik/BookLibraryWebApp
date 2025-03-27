using System.Collections.Generic;
using System.Threading.Tasks;
using BookLibraryDataAccessClassLibrary.Models;

namespace BookLibraryDataAccessClassLibrary.Interfaces
{
    public interface IAllUsers
    {
        Task<IEnumerable<User>> GetAllUsersAsync(CancellationToken cancellationToken);
        Task<User> GetUserAsync(int id, CancellationToken cancellationToken);
        Task CreateAsync(User user, CancellationToken cancellationToken);
        Task UpdateAsync(User user, CancellationToken cancellationToken);
        Task DeleteAsync(User user, CancellationToken cancellationToken);
    }
}