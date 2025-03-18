using System.Collections.Generic;
using System.Linq;
using BookLibraryAPI.Interfaces;
using BookLibraryAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace BookLibraryAPI.Services
{
    public class CountryService : IAllCountries
    {
        private readonly Db15460Context _context;

        public CountryService(Db15460Context context)
        {
            _context = context;
        }

        public IEnumerable<Country> AllCountries => _context.Countries;

        public Country GetById(int id)
        {
            return _context.Countries.FirstOrDefault(c => c.Id == id) ??
                throw new InvalidOperationException($"Страна с ID {id} не найдена.");
        }

        public void Create(Country country)
        {
            _context.Countries.Add(country);
            _context.SaveChanges();
        }

        public void Update(Country country)
        {
            _context.Countries.Update(country);
            _context.SaveChanges();
        }

        public void Delete(Country country)
        {
            _context.Countries.Remove(country);
            _context.SaveChanges();
        }
    }
}