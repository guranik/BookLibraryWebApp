using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using BookLibraryBusinessLogicClassLibrary.DTOs.Authors;
using BookLibraryBusinessLogicClassLibrary.DTOs.PagedResult;
using BookLibraryDataAccessClassLibrary.Interfaces;
using BookLibraryDataAccessClassLibrary.Models;
using BookLibraryBusinessLogicClassLibrary.Services;
using Moq;
using Xunit;

namespace BookLubraryServiceTests
{
    public class AuthorServiceTests
    {
        private readonly Mock<IAllAuthors> _authorRepositoryMock;
        private readonly IMapper _mapper;
        private readonly AuthorService _authorService;

        public AuthorServiceTests()
        {
            _authorRepositoryMock = new Mock<IAllAuthors>();

            var mappingConfig = new MapperConfiguration(mc =>
            {
                mc.CreateMap<Author, AuthorDto>().ReverseMap();
            });
            _mapper = mappingConfig.CreateMapper();

            _authorService = new AuthorService(_authorRepositoryMock.Object, _mapper);
        }

        [Fact]
        public async Task GetAllAuthorsAsync_ShouldReturnListOfAuthors()
        {
            // Arrange
            var authors = new List<Author> { new Author { Id = 1, Name = "Author 1" } };
            _authorRepositoryMock.Setup(repo => repo.GetAllAuthorsAsync()).ReturnsAsync(authors);

            // Act
            var result = await _authorService.GetAllAuthorsAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal("Author 1", result[0].Name);
        }



        [Fact]
        public async Task GetAuthorByIdAsync_ShouldReturnAuthor()
        {
            // Arrange
            var author = new Author { Id = 1, Name = "Author 1" };
            _authorRepositoryMock.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync(author);

            // Act
            var result = await _authorService.GetAuthorByIdAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Author 1", result.Name);
        }

        [Fact]
        public async Task CreateAuthorAsync_ShouldAddAuthor()
        {
            // Arrange
            var authorDto = new AuthorDto { Name = "Author 1" };
            _authorRepositoryMock.Setup(repo => repo.CreateAsync(It.IsAny<Author>())).Returns(Task.CompletedTask);

            // Act
            await _authorService.CreateAuthorAsync(authorDto);

            // Assert
            _authorRepositoryMock.Verify(repo => repo.CreateAsync(It.IsAny<Author>()), Times.Once);
        }

        [Fact]
        public async Task UpdateAuthorAsync_ShouldUpdateAuthor()
        {
            // Arrange
            var authorDto = new AuthorDto { Id = 1, Name = "Updated Author" };
            _authorRepositoryMock.Setup(repo => repo.UpdateAsync(It.IsAny<Author>())).Returns(Task.CompletedTask);

            // Act
            await _authorService.UpdateAuthorAsync(authorDto);

            // Assert
            _authorRepositoryMock.Verify(repo => repo.UpdateAsync(It.IsAny<Author>()), Times.Once);
        }

        [Fact]
        public async Task DeleteAuthorAsync_ShouldDeleteAuthor()
        {
            // Arrange
            var author = new Author { Id = 1, Name = "Author 1" };
            _authorRepositoryMock.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync(author);
            _authorRepositoryMock.Setup(repo => repo.DeleteAsync(author)).Returns(Task.CompletedTask);

            // Act
            await _authorService.DeleteAuthorAsync(1);

            // Assert
            _authorRepositoryMock.Verify(repo => repo.DeleteAsync(author), Times.Once);
        }

        [Fact]
        public async Task DeleteAuthorAsync_ShouldNotDeleteNonExistentAuthor()
        {
            // Arrange
            _authorRepositoryMock.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync((Author)null);

            // Act
            await _authorService.DeleteAuthorAsync(1);

            // Assert
            _authorRepositoryMock.Verify(repo => repo.DeleteAsync(It.IsAny<Author>()), Times.Never);
        }
    }
}