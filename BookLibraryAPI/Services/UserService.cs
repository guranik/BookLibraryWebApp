using BookLibraryAPI.Interfaces;
using BookLibraryAPI.Models;

namespace BookLibraryAPI.Services
{
    public class UserService : IAllUsers
    {
        private readonly Db15460Context _context;
        public UserService(Db15460Context context)
        {
            _context = context;
        }

        public IEnumerable<User> AllUsers => _context.Users;

        public User GetUser(int id) => _context.Users.FirstOrDefault(x => x.Id == id)
            ?? throw new InvalidOperationException($"User  with ID {id} not found.");

        public void Create(User user)
        {
            _context.Users.Add(user);
            _context.SaveChanges();
        }

        public void Update(User user)
        {
            _context.Users.Update(user);
            _context.SaveChanges();
        }

        public void Delete(User user)
        {
            _context.Users.Remove(user);
            _context.SaveChanges();
        }
    }
}
