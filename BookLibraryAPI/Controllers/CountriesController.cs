using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BookLibraryAPI.Interfaces;
using BookLibraryAPI.DTOs.Countries;
using System.Collections.Generic;
using System.Threading.Tasks;
using BookLibraryAPI.Services;

namespace BookLibraryAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CountriesController : ControllerBase
    {
        private readonly ICountryService _countryService;

        public CountriesController(ICountryService countryService)
        {
            _countryService = countryService;
        }

        [HttpGet("all")]
        public async Task<ActionResult<IEnumerable<CountryDto>>> GetAllCountries()
        {
            var countryDtos = await _countryService.GetAllCountriesAsync();
            return Ok(countryDtos);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CountryDto>> GetCountryById(int id)
        {
            var countryDto = await _countryService.GetCountryByIdAsync(id);
            if (countryDto == null)
            {
                return NotFound();
            }
            return Ok(countryDto);
        }

        [Authorize(Policy = "AdminOnly")]
        [HttpPost]
        public async Task<ActionResult<CountryDto>> CreateCountry([FromBody] CountryDto countryDto)
        {
            if (countryDto == null)
            {
                return BadRequest("Country cannot be null.");
            }

            await _countryService.CreateCountryAsync(countryDto);
            return CreatedAtRoute(nameof(GetCountryById), new { id = countryDto.Id }, countryDto);
        }

        [Authorize(Policy = "AdminOnly")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCountry(int id, [FromBody] CountryDto countryDto)
        {
            if (countryDto == null || countryDto.Id != id)
            {
                return BadRequest("Country data is invalid.");
            }

            try
            {
                await _countryService.UpdateCountryAsync(countryDto);
                return NoContent();
            }
            catch (InvalidOperationException)
            {
                return NotFound();
            }
        }

        [Authorize(Policy = "AdminOnly")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCountry(int id)
        {
            var country = await _countryService.GetCountryByIdAsync(id);
            if (country == null)
            {
                return NotFound();
            }

            await _countryService.DeleteCountryAsync(id);
            return NoContent();
        }
    }
}