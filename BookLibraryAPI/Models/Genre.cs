namespace BookLibraryAPI.Models
{
    public class Genre
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public virtual ICollection<Book> Books { get; set; } = new List<Book>();

        public override string ToString()
        {
            return $"{Name}";
        }
    }
}
