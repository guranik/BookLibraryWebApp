namespace BookLibraryAPI.DTOs.Authors
{
    public class AuthorDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Surname { get; set; } = null!;
    }
}