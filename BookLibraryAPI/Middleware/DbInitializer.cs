using BookLibraryAPI.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BookLibraryAPI.Middleware
{
    public class DbInitializer
    {
        public static void Initialize(Db15460Context context)
        {
            context.Database.EnsureCreated();

            if(context.Books.Any())
            {
                return;
            }

            // Инициализация стран
            var countries = new List<Country>
            {
                new Country { Name = "Россия" },
                new Country { Name = "США" },
                new Country { Name = "Франция" }
            };
            context.Countries.AddRange(countries);
            context.SaveChanges();

            // Инициализация жанров
            var genres = new List<Genre>
            {
                new Genre { Name = "Фантастика" },
                new Genre { Name = "Драма" },
                new Genre { Name = "Приключения" }
            };
            context.Genres.AddRange(genres);
            context.SaveChanges();

            // Инициализация авторов
            var authors = new List<Author>
            {
                new Author { Name = "Лев", Surname = "Толстой", BirthDate = new DateTime(1828, 9, 9), CountryId = countries[0].Id },
                new Author { Name = "Фёдор", Surname = "Достоевский", BirthDate = new DateTime(1821, 11, 11), CountryId = countries[0].Id },
                new Author { Name = "Эрнест", Surname = "Хемингуэй", BirthDate = new DateTime(1899, 7, 21), CountryId = countries[1].Id }
            };
            context.Authors.AddRange(authors);
            context.SaveChanges();

            // Инициализация книг
            var books = new List<Book>
            {
                new Book { ISBN = "978-3-16-148410-0", Title = "Война и мир", Description = "Роман о жизни русского общества в начале 19 века", AuthorId = authors[0].Id, GenreId = genres[1].Id, BookNumber = 1 },
                new Book { ISBN = "978-3-16-148410-1", Title = "Преступление и наказание", Description = "Роман о моральных дилеммах и искуплении", AuthorId = authors[1].Id, GenreId = genres[1].Id, BookNumber = 2 },
                new Book { ISBN = "978-3-16-148410-2", Title = "Старик и море", Description = "Классическая повесть о борьбе человека с природой", AuthorId = authors[2].Id, GenreId = genres[0].Id, BookNumber = 3 }
            };
            context.Books.AddRange(books);
            context.SaveChanges();
        }
    }
}