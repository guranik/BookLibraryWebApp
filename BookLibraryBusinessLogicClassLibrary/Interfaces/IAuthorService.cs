using BookLibraryBusinessLogicClassLibrary.DTOs.Authors;

namespace BookLibraryBusinessLogicClassLibrary.Interfaces
{
    public interface IAuthorService
    {
        Task<List<AuthorDto>> GetAllAuthorsAsync(CancellationToken cancellationToken);
        Task<AuthorDto> GetAuthorByIdAsync(int id, CancellationToken cancellationToken);
        Task CreateAuthorAsync(AuthorInfoDto authorDto, CancellationToken cancellationToken);
        Task UpdateAuthorAsync(AuthorInfoDto authorDto, CancellationToken cancellationToken);
        Task DeleteAuthorAsync(int id, CancellationToken cancellationToken);
    }
}