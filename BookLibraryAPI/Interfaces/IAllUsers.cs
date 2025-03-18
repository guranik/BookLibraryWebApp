using BookLibraryAPI.Models;

namespace BookLibraryAPI.Interfaces
{
    public interface IAllUsers
    {
        IEnumerable<User> AllUsers { get; }
        User GetUser(int id);
        void Create(User user);
        void Update(User user);
        void Delete(User user);
    }
}
