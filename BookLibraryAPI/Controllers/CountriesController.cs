using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BookLibraryDataAccessClassLibrary.Interfaces;
using BookLibraryBusinessLogicClassLibrary.DTOs.Countries;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using BookLibraryBusinessLogicClassLibrary.Services;

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
        public async Task<ActionResult<IEnumerable<CountryDto>>> GetAllCountries(CancellationToken cancellationToken)
        {
            var countryDtos = await _countryService.GetAllCountriesAsync(cancellationToken);
            return Ok(countryDtos);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CountryDto>> GetCountryById(int id, CancellationToken cancellationToken)
        {
            var countryDto = await _countryService.GetCountryByIdAsync(id, cancellationToken);
            if (countryDto == null)
            {
                return NotFound();
            }
            return Ok(countryDto);
        }

        [Authorize(Policy = "AdminOnly")]
        [HttpPost]
        public async Task<ActionResult<CountryDto>> CreateCountry([FromBody] CountryDto countryDto, CancellationToken cancellationToken)
        {
            if (countryDto == null)
            {
                return BadRequest("Country cannot be null.");
            }

            await _countryService.CreateCountryAsync(countryDto, cancellationToken);
            return CreatedAtRoute(nameof(GetCountryById), new { id = countryDto.Id }, countryDto);
        }

        [Authorize(Policy = "AdminOnly")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCountry(int id, [FromBody] CountryDto countryDto, CancellationToken cancellationToken)
        {
            if (countryDto == null || countryDto.Id != id)
            {
                return BadRequest("Country data is invalid.");
            }
            await _countryService.UpdateCountryAsync(countryDto, cancellationToken);
            return NoContent();
        }

        [Authorize(Policy = "AdminOnly")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCountry(int id, CancellationToken cancellationToken)
        {
            var country = await _countryService.GetCountryByIdAsync(id, cancellationToken);
            if (country == null)
            {
                return NotFound();
            }

            await _countryService.DeleteCountryAsync(id, cancellationToken);
            return NoContent();
        }
    }
}