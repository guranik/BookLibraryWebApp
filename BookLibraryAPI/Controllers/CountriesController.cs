using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BookLibraryDataAccessClassLibrary.Interfaces;
using BookLibraryBusinessLogicClassLibrary.DTOs.Countries;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using BookLibraryBusinessLogicClassLibrary.Interfaces;

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
            return Ok(await _countryService.GetCountryByIdAsync(id, cancellationToken));
        }

        [Authorize(Policy = "AdminOnly")]
        [HttpPost]
        public async Task<ActionResult<CountryDto>> CreateCountry([FromBody] CountryDto countryDto, CancellationToken cancellationToken)
        {
            await _countryService.CreateCountryAsync(countryDto, cancellationToken);
            return CreatedAtRoute(nameof(GetCountryById), new { id = countryDto.Id }, countryDto);
        }

        [Authorize(Policy = "AdminOnly")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCountry(int id, [FromBody] CountryDto countryDto, CancellationToken cancellationToken)
        {
            countryDto.Id = id; // Set the ID from the route
            await _countryService.UpdateCountryAsync(countryDto, cancellationToken);
            return NoContent();
        }

        [Authorize(Policy = "AdminOnly")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCountry(int id, CancellationToken cancellationToken)
        {
            await _countryService.DeleteCountryAsync(id, cancellationToken);
            return NoContent();
        }
    }
}