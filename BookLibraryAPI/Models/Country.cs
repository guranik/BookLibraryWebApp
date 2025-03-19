namespace BookLibraryAPI.Models
{
    public class Country
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public virtual ICollection<Author> Authors { get; set; } = new List<Author>();
        public override string ToString()
        {
            return $"{Name}";
        }
    }
}
