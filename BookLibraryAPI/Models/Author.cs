namespace BookLibraryAPI.Models
{
    public class Author
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Surname { get; set; } = null!;
        public DateTime BirthDate { get; set; }
        public int CountryId { get; set; }
        public virtual ICollection<Book> Books { get; set; } = new List<Book>();
        public override string ToString()
        {
            return $"{Name} {Surname}";
        }
    }
}
