using BookLibraryAPI.DTOs.Countries;

namespace BookLibraryAPI.DTOs.Authors
{
    public class AuthorInfoDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Surname { get; set; } = null!;
        public DateTime BirthDate { get; set; }
        public int CountryId { get; set; }
        public CountryDto? Country { get; set; }
    }
}
