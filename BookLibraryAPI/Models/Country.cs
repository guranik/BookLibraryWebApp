namespace BookLibraryAPI.Models
{
    public class Country
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public virtual ICollection<Country> Authors { get; set; } = new List<Country>();
        public override string ToString()
        {
            return $"{Name}";
        }
    }
}
