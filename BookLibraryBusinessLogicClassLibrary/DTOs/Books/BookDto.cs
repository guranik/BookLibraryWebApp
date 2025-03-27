
namespace BookLibraryBusinessLogicClassLibrary.DTOs.Books  
{
    public class BookDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public string Author { get; set; } = null!;
        public string Genre { get; set; } = null!;
        public int BookNumber { get; set; }
    }
}
