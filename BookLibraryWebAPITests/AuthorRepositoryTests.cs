using BookLibraryAPI.Models;
using BookLibraryAPI.Services;
using Microsoft.EntityFrameworkCore;

public class AuthorRepositoryTests
{
    private readonly AuthorRepository _service;
    private readonly DbContextOptions<Db15460Context> _options;

    public AuthorRepositoryTests()
    {
        // �������� ���������� in-memory ���� ������
        _options = new DbContextOptionsBuilder<Db15460Context>()
            .UseInMemoryDatabase(databaseName: "TestDatabase")
            .Options;

        // ������������� �������
        _service = new AuthorRepository(new Db15460Context(_options));
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsAuthor_WhenAuthorExists()
    {
        // ����������: ���������� ������ � ���� ������
        using (var context = new Db15460Context(_options))
        {
            var author = new Author { Id = 1, Name = "John", Surname = "Doe" };
            context.Authors.Add(author);
            await context.SaveChangesAsync();
        }

        // ��������: ��������� ������ �� ID
        var result = await _service.GetByIdAsync(1);

        // ��������: ���������, ��� ��������� �� ����� null � ��������� � ���������
        Assert.NotNull(result);
        Assert.Equal("John", result.Name);
        Assert.Equal("Doe", result.Surname);
    }

    [Fact]
    public async Task CreateAsync_AddsAuthor()
    {
        // �������� ���� ������ ����� ������
        using (var context = new Db15460Context(_options))
        {
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();
        }

        var author = new Author { Id = 2, Name = "Jane", Surname = "Smith" };
        await _service.CreateAsync(author);

        using (var context = new Db15460Context(_options))
        {
            var addedAuthor = await context.Authors.FindAsync(2);
            Assert.NotNull(addedAuthor);
            Assert.Equal("Jane", addedAuthor.Name);
        }
    }
}