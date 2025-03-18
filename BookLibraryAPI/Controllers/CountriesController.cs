using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BookLibraryAPI.Interfaces;
using BookLibraryAPI.Models;
using System.Collections.Generic;

namespace BookLibraryAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class CountriesController : ControllerBase
    {
        private readonly IAllCountries _countryService;

        public CountriesController(IAllCountries countryService)
        {
            _countryService = countryService;
        }

        // GET: api/countries
        [HttpGet]
        public ActionResult<IEnumerable<Country>> GetAllCountries()
        {
            var countries = _countryService.AllCountries;
            return Ok(countries);
        }

        // GET: api/countries/{id}
        [HttpGet("{id}", Name = "GetCountryById")]
        public ActionResult<Country> GetCountryById(int id)
        {
            try
            {
                var country = _countryService.GetById(id);
                return Ok(country);
            }
            catch (InvalidOperationException)
            {
                return NotFound();
            }
        }

        // POST: api/countries
        [Authorize]
        [Authorize(Policy = "AdminOnly")]
        [HttpPost]
        public ActionResult<Country> CreateCountry([FromBody] Country country)
        {
            if (country == null)
            {
                return BadRequest("Country cannot be null.");
            }

            _countryService.Create(country);
            return CreatedAtRoute("GetCountryById", new { id = country.Id }, country);
        }

        // PUT: api/countries/{id}
        [Authorize]
        [Authorize(Policy = "AdminOnly")]
        [HttpPut("{id}")]
        public IActionResult UpdateCountry(int id, [FromBody] Country country)
        {
            if (country == null || country.Id != id)
            {
                return BadRequest("Country data is invalid.");
            }

            try
            {
                _countryService.Update(country);
                return NoContent();
            }
            catch (InvalidOperationException)
            {
                return NotFound();
            }
        }

        // DELETE: api/countries/{id}
        [Authorize]
        [Authorize(Policy = "AdminOnly")]
        [HttpDelete("{id}")]
        public IActionResult DeleteCountry(int id)
        {
            var country = _countryService.GetById(id);
            if (country == null)
            {
                return NotFound();
            }

            _countryService.Delete(country);
            return NoContent();
        }
    }
}