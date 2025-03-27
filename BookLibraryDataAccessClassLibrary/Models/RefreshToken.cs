using System;

namespace BookLibraryDataAccessClassLibrary.Models
{
    public class RefreshToken
    {
        public int Id { get; set; }
        public string Token { get; set; } = null!;
        public int UserId { get; set; }
        public DateTime ExpiryDate { get; set; }
        public bool IsRevoked { get; set; } = false;

        public virtual User User { get; set; } = null!;
    }
}