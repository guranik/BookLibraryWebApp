using BookLibraryBusinessLogicClassLibrary.DTOs.Authors;

namespace BookLibraryBusinessLogicClassLibrary.Interfaces
{
    public interface IAuthorService
    {
        Task<List<AuthorDto>> GetAllAuthorsAsync(CancellationToken cancellationToken);
        Task<AuthorDto> GetAuthorByIdAsync(int id, CancellationToken cancellationToken);
        Task CreateAuthorAsync(AuthorDto authorDto, CancellationToken cancellationToken);
        Task UpdateAuthorAsync(AuthorDto authorDto, CancellationToken cancellationToken);
        Task DeleteAuthorAsync(int id, CancellationToken cancellationToken);
    }
}