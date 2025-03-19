using AutoMapper;
using BookLibraryAPI.Dtos.Books;
using BookLibraryAPI.Models;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Book, BookDto>()
            .ForMember(dest => dest.Genre, opt => opt.MapFrom(src => src.Genre.Name));
        CreateMap<Author, AuthorDto>();
    }
}