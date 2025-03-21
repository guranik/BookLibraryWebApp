using AutoMapper;
using BookLibraryAPI.Controllers;
using BookLibraryAPI.DTOs.Authors;
using BookLibraryAPI.DTOs.Books;
using BookLibraryAPI.DTOs.Countries;
using BookLibraryAPI.DTOs.Genres;
using BookLibraryAPI.DTOs.IssuedBooks;
using BookLibraryAPI.DTOs.Users;
using BookLibraryAPI.Models;

namespace BookLibraryAPI.Middleware
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Author, AuthorDto>();

            CreateMap<Author, AuthorInfoDto>()
                .ForMember(dest => dest.CountryId, opt => opt.MapFrom(src => src.CountryId))
                .AfterMap((src, dest) =>
                {
                    dest.Country = new CountryDto
                    {
                        Id = src.CountryId,
                        Name = src.Country.Name
                    };
                });
            CreateMap<AuthorInfoDto, Author>()
                .ForMember(dest => dest.CountryId, opt => opt.MapFrom(src => src.CountryId));

            CreateMap<Book, BookDto>()
                .ForMember(dest => dest.Author, opt => opt.MapFrom(src => $"{src.Author.Name} {src.Author.Surname}"))
                .ForMember(dest => dest.Genre, opt => opt.MapFrom(src => src.Genre.Name));

            CreateMap<Book, BookInfoDto>()
                .ForMember(dest => dest.Author, opt => opt.MapFrom(src => new AuthorDto
                {
                    Id = src.AuthorId,
                    Name = src.Author.Name,
                    Surname = src.Author.Surname
                }))
                .ForMember(dest => dest.Genre, opt => opt.MapFrom(src => new GenreDto
                {
                    Id = src.GenreId,
                    Name = src.Genre.Name
                }));
            CreateMap<BookInfoDto, Book>()
                .ForMember(dest => dest.AuthorId, opt => opt.MapFrom(src => src.AuthorId))
                .ForMember(dest => dest.GenreId, opt => opt.MapFrom(src => src.GenreId));

            CreateMap<IssuedBook, IssuedBookDto>()
                .ForMember(dest => dest.Book, opt => opt.MapFrom(src => new BookDto
                {
                    Id = src.BookId,
                    Title = src.Book.Title,
                    Author = $"{src.Book.Author.Name} {src.Book.Author.Surname}",
                    Genre = src.Book.Genre.Name,
                    BookNumber = src.Book.BookNumber
                }));
            CreateMap<IssuedBookDto, IssuedBook>();
            
            CreateMap<User, UserDto>();

            CreateMap<RegisterModel, User>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Login));
            CreateMap<CountryDto, Country>();
            CreateMap<Country, CountryDto>();
            CreateMap<Genre, GenreDto>();
            CreateMap<GenreDto, Genre>();
        }
    }
}