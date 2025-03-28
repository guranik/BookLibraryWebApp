﻿using Microsoft.AspNetCore.Identity;

namespace BookLibraryDataAccessClassLibrary.Models;

public partial class User : IdentityUser<int>
{
    public string Login { get; set; } = null!;
    public virtual ICollection<IssuedBook> IssuedBooks { get; set; } = new List<IssuedBook>();
}

