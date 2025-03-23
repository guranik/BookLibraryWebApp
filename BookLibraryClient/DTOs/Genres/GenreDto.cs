namespace BookLibraryClient.DTOs.Genres
{
    public class GenreDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;

        public override string ToString() => Name;
    }
}
