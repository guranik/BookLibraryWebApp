namespace BookLibraryAPI.DTOs.Authors
{
    public class AuthorInfoDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Surname { get; set; } = null!;
        public DateTime BirthDate { get; set; }
        public string CountryName { get; set; } = null!;
    }
}
