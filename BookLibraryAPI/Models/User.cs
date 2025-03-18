using Microsoft.AspNetCore.Identity;

namespace BookLibraryAPI.Models;

public partial class User : IdentityUser<int>
{
    public string Login { get; set; } = null!;
    public virtual ICollection<Book> Books { get; set; } = new List<Book>();
}

