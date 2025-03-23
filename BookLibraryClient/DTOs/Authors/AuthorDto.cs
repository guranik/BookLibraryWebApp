using System.Security.Cryptography.X509Certificates;

namespace BookLibraryClient.DTOs.Authors
{
    public class AuthorDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Surname { get; set; } = null!;

        public override string ToString()
        {
            return $"{Name} {Surname}";
        }
    }
}
