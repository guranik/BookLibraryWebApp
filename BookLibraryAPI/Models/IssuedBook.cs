namespace BookLibraryAPI.Models
{
    public class IssuedBook
    {
        public int Id { get; set; }
        public int BookId { get; set; }
        public Book? Book { get; set; }
        public int UserId { get; set; }
        public User? User { get; set; }
        public DateTime? Issued { get; set; }
        public DateTime? Return { get; set; }
    }
}
