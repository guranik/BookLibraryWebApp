using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using BookLibraryAPI.Interfaces;
using BookLibraryAPI.Models;
using BookLibraryAPI.DTOs.Authors;
using BookLibraryAPI.DTOs.PagedResult;

namespace BookLibraryAPI.Services
{
    public interface IAuthorService
    {
        Task<List<AuthorDto>> GetAllAuthorsAsync();
        Task<PagedAuthorsDto> GetPagedAuthorsAsync(int pageNumber, int pageSize);
        Task<List<AuthorDto>> GetSortedAuthorsAsync();
        Task<AuthorDto> GetAuthorByIdAsync(int id);
        Task CreateAuthorAsync(AuthorDto authorDto);
        Task UpdateAuthorAsync(AuthorDto authorDto);
        Task DeleteAuthorAsync(int id);
    }

    public class AuthorService : IAuthorService
    {
        private readonly IAllAuthors _authorRepository;
        private readonly IMapper _mapper;

        public AuthorService(IAllAuthors authorRepository, IMapper mapper)
        {
            _authorRepository = authorRepository;
            _mapper = mapper;
        }

        public async Task<List<AuthorDto>> GetAllAuthorsAsync()
        {
            var authors = await _authorRepository.GetAllAuthorsAsync();
            return _mapper.Map<List<AuthorDto>>(authors);
        }

        public async Task<PagedAuthorsDto> GetPagedAuthorsAsync(int pageNumber, int pageSize)
        {
            var pagedAuthors = await _authorRepository.GetPagedAuthorsAsync(pageNumber, pageSize);
            var authorDtos = _mapper.Map<List<AuthorDto>>(pagedAuthors.Items);
            return new PagedAuthorsDto
            {
                Items = authorDtos,
                TotalPages = pagedAuthors.TotalPages,
                CurrentPage = pagedAuthors.PageNumber
            };
        }

        public async Task<List<AuthorDto>> GetSortedAuthorsAsync()
        {
            var authors = await _authorRepository.GetSortedAuthorsAsync();
            return _mapper.Map<List<AuthorDto>>(authors);
        }

        public async Task<AuthorDto> GetAuthorByIdAsync(int id)
        {
            var author = await _authorRepository.GetByIdAsync(id);
            return _mapper.Map<AuthorDto>(author);
        }

        public async Task CreateAuthorAsync(AuthorDto authorDto)
        {
            var author = _mapper.Map<Author>(authorDto);
            await _authorRepository.CreateAsync(author);
        }

        public async Task UpdateAuthorAsync(AuthorDto authorDto)
        {
            var author = _mapper.Map<Author>(authorDto);
            await _authorRepository.UpdateAsync(author);
        }

        public async Task DeleteAuthorAsync(int id)
        {
            var author = await _authorRepository.GetByIdAsync(id);
            if (author != null)
            {
                await _authorRepository.DeleteAsync(author);
            }
        }
    }
}