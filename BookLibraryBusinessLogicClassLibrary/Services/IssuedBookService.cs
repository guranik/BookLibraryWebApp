using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using BookLibraryDataAccessClassLibrary.Interfaces;
using BookLibraryDataAccessClassLibrary.Models;
using BookLibraryBusinessLogicClassLibrary.DTOs.IssuedBooks;
using BookLibraryBusinessLogicClassLibrary.DTOs.PagedResult;

namespace BookLibraryBusinessLogicClassLibrary.Services
{
    public interface IIssuedBookService
    {
        Task<PagedIssuedBooksDto> GetByUserAsync(int userId, int pageNumber, int pageSize, CancellationToken cancellationToken);
        Task<IssuedBookDto> GetByIdAsync(int id, CancellationToken cancellationToken);
        Task CreateAsync(IssuedBookDto issuedBookDto, CancellationToken cancellationToken);
        Task UpdateAsync(int id, IssuedBookDto issuedBookDto, CancellationToken cancellationToken);
        Task DeleteAsync(int id, CancellationToken cancellationToken);
    }

    public class IssuedBookService : IIssuedBookService
    {
        private readonly IAllIssuedBooks _issuedBookRepository;
        private readonly IMapper _mapper;

        public IssuedBookService(IAllIssuedBooks issuedBookRepository, IMapper mapper)
        {
            _issuedBookRepository = issuedBookRepository;
            _mapper = mapper;
        }

        public async Task<PagedIssuedBooksDto> GetByUserAsync(int userId, int pageNumber, int pageSize, CancellationToken cancellationToken)
        {
            var issuedBooks = await _issuedBookRepository.GetByUserAsync(userId, pageNumber, pageSize, cancellationToken);
            var issuedBookDtos = _mapper.Map<List<IssuedBookDto>>(issuedBooks.Items);
            return new PagedIssuedBooksDto
            {
                Items = issuedBookDtos,
                TotalPages = issuedBooks.TotalPages,
                CurrentPage = issuedBooks.PageNumber
            };
        }

        public async Task<IssuedBookDto> GetByIdAsync(int id, CancellationToken cancellationToken)
        {
            var issuedBook = await _issuedBookRepository.GetByIdAsync(id, cancellationToken);
            return _mapper.Map<IssuedBookDto>(issuedBook);
        }

        public async Task CreateAsync(IssuedBookDto issuedBookDto, CancellationToken cancellationToken)
        {
            var issuedBook = _mapper.Map<IssuedBook>(issuedBookDto);
            await _issuedBookRepository.CreateAsync(issuedBook, cancellationToken);
        }

        public async Task UpdateAsync(int id, IssuedBookDto issuedBookDto, CancellationToken cancellationToken)
        {
            var issuedBook = _mapper.Map<IssuedBook>(issuedBookDto);
            await _issuedBookRepository.UpdateAsync(issuedBook, cancellationToken);
        }

        public async Task DeleteAsync(int id, CancellationToken cancellationToken)
        {
            var issuedBook = await _issuedBookRepository.GetByIdAsync(id, cancellationToken);
            if (issuedBook != null)
            {
                await _issuedBookRepository.DeleteAsync(issuedBook, cancellationToken);
            }
        }
    }
}