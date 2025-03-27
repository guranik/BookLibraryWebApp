﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookLibraryDataAccessClassLibrary.Interfaces;
using BookLibraryDataAccessClassLibrary.Models;
using Microsoft.EntityFrameworkCore;

namespace BookLibraryDataAccessClassLibrary.Repositories
{
    public class AuthorRepository : IAllAuthors
    {
        private readonly Db15460Context _context;

        public AuthorRepository(Db15460Context context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Author>> GetAllAuthorsAsync()
            => await _context.Authors.ToListAsync();

        public async Task<Author> GetByIdAsync(int id)
        {
            return await _context.Authors.Include(a => a.Books).FirstOrDefaultAsync(a => a.Id == id)
                ?? throw new InvalidOperationException($"Автор с ID {id} не найден.");
        }

        public async Task CreateAsync(Author author)
        {
            if (!await AuthorExistsAsync(author))
            {
                await _context.Authors.AddAsync(author);
                await _context.SaveChangesAsync();
            }
            else
            {
                throw new InvalidOperationException("Автор с такими данными уже существует.");
            }
        }

        public async Task UpdateAsync(Author author)
        {
            var existingAuthor = await GetByIdAsync(author.Id);
            if (existingAuthor != null)
            {
                existingAuthor.Name = author.Name;
                existingAuthor.Surname = author.Surname;
                existingAuthor.BirthDate = author.BirthDate;
                existingAuthor.CountryId = author.CountryId;

                _context.Authors.Update(existingAuthor);
                await _context.SaveChangesAsync();
            }
            else
            {
                throw new InvalidOperationException($"Автор с ID {author.Id} не найден.");
            }
        }

        public async Task DeleteAsync(Author author)
        {
            var existingAuthor = await GetByIdAsync(author.Id);
            if (existingAuthor != null)
            {
                _context.Authors.Remove(existingAuthor);
                await _context.SaveChangesAsync();
            }
            else
            {
                throw new InvalidOperationException($"Автор с ID {author.Id} не найден.");
            }
        }

        private async Task<bool> AuthorExistsAsync(Author author)
        {
            return await _context.Authors.AnyAsync(a =>
                a.Id == author.Id ||
                (a.Name == author.Name &&
                 a.Surname == author.Surname &&
                 a.BirthDate == author.BirthDate &&
                 a.CountryId == author.CountryId));
        }
    }
}