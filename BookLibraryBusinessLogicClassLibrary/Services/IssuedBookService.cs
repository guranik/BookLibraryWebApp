using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using BookLibraryDataAccessClassLibrary.Interfaces;
using BookLibraryDataAccessClassLibrary.Models;
using BookLibraryBusinessLogicClassLibrary.DTOs.IssuedBooks;
using BookLibraryBusinessLogicClassLibrary.DTOs.PagedResult;
using BookLibraryBusinessLogicClassLibrary.Interfaces;
using BookLibraryBusinessLogicClassLibrary.Exceptions;

namespace BookLibraryBusinessLogicClassLibrary.Services
{
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
            if (issuedBook == null)
            {
                throw new NotFoundException($"Issued book with ID {id} not found.");
            }
            return _mapper.Map<IssuedBookDto>(issuedBook);
        }

        public async Task CreateAsync(IssuedBookDto issuedBookDto, CancellationToken cancellationToken)
        {
            if (issuedBookDto == null)
            {
                throw new BadRequestException("Issued Book cannot be null.");
            }

            var issuedBook = _mapper.Map<IssuedBook>(issuedBookDto);
            await _issuedBookRepository.CreateAsync(issuedBook, cancellationToken);
        }

        public async Task UpdateAsync(int id, IssuedBookDto issuedBookDto, CancellationToken cancellationToken)
        {
            if (issuedBookDto == null || issuedBookDto.Id != id)
            {
                throw new BadRequestException("Issued Book data is invalid.");
            }

            var existingIssuedBook = await _issuedBookRepository.GetByIdAsync(id, cancellationToken);
            if (existingIssuedBook == null)
            {
                throw new NotFoundException($"Issued book with ID {id} not found.");
            }

            existingIssuedBook.BookId = issuedBookDto.BookId;
            existingIssuedBook.Issued = issuedBookDto.Issued;
            existingIssuedBook.Return = issuedBookDto.Return;

            await _issuedBookRepository.UpdateAsync(existingIssuedBook, cancellationToken);
        }

        public async Task DeleteAsync(int id, CancellationToken cancellationToken)
        {
            var issuedBook = await _issuedBookRepository.GetByIdAsync(id, cancellationToken);
            if (issuedBook == null)
            {
                throw new NotFoundException($"Issued book with ID {id} not found.");
            }
            await _issuedBookRepository.DeleteAsync(issuedBook, cancellationToken);
        }
    }
}