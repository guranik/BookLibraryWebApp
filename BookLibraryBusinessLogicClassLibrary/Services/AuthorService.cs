using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using BookLibraryDataAccessClassLibrary.Interfaces;
using BookLibraryDataAccessClassLibrary.Models;
using BookLibraryBusinessLogicClassLibrary.DTOs.Authors;
using BookLibraryBusinessLogicClassLibrary.Exceptions;
using BookLibraryBusinessLogicClassLibrary.Interfaces;

namespace BookLibraryBusinessLogicClassLibrary.Services
{
    public class AuthorService : IAuthorService
    {
        private readonly IAllAuthors _authorRepository;
        private readonly IMapper _mapper;

        public AuthorService(IAllAuthors authorRepository, IMapper mapper)
        {
            _authorRepository = authorRepository;
            _mapper = mapper;
        }

        public async Task<List<AuthorDto>> GetAllAuthorsAsync(CancellationToken cancellationToken)
        {
            var authors = await _authorRepository.GetAllAuthorsAsync(cancellationToken);
            return _mapper.Map<List<AuthorDto>>(authors);
        }

        public async Task<AuthorDto> GetAuthorByIdAsync(int id, CancellationToken cancellationToken)
        {
            var author = await _authorRepository.GetByIdAsync(id, cancellationToken);
            if (author == null)
            {
                throw new NotFoundException($"Author with ID {id} not found.");
            }
            return _mapper.Map<AuthorDto>(author);
        }

        public async Task CreateAuthorAsync(AuthorDto authorDto, CancellationToken cancellationToken)
        {
            var author = _mapper.Map<Author>(authorDto);
            await _authorRepository.CreateAsync(author, cancellationToken);
        }

        public async Task UpdateAuthorAsync(AuthorDto authorDto, CancellationToken cancellationToken)
        {
            var existingAuthor = await _authorRepository.GetByIdAsync(authorDto.Id, cancellationToken);
            if (existingAuthor == null)
            {
                throw new NotFoundException($"Author with ID {authorDto.Id} not found.");
            }

            var author = _mapper.Map<Author>(authorDto);
            await _authorRepository.UpdateAsync(author, cancellationToken);
        }

        public async Task DeleteAuthorAsync(int id, CancellationToken cancellationToken)
        {
            var author = await _authorRepository.GetByIdAsync(id, cancellationToken);
            if (author == null)
            {
                throw new NotFoundException($"Author with ID {id} not found.");
            }

            await _authorRepository.DeleteAsync(author, cancellationToken);
        }
    }
}