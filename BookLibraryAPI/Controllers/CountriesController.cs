using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BookLibraryAPI.Interfaces;
using BookLibraryAPI.Models;
using BookLibraryAPI.DTOs.Countries;
using AutoMapper;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BookLibraryAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CountriesController : ControllerBase
    {
        private readonly IAllCountries _countryService;
        private readonly IMapper _mapper;

        public CountriesController(IAllCountries countryService, IMapper mapper)
        {
            _countryService = countryService;
            _mapper = mapper;
        }

        [HttpGet("all")]
        public async Task<ActionResult<IEnumerable<CountryDto>>> GetAllCountries()
        {
            var countries = await _countryService.GetAllCountriesAsync();
            var countryDtos = _mapper.Map<List<CountryDto>>(countries);
            return Ok(countryDtos);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CountryDto>> GetCountryById(int id)
        {
            var country = await _countryService.GetByIdAsync(id);
            if (country == null)
            {
                return NotFound();
            }
            var countryDto = _mapper.Map<CountryDto>(country);
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

            var country = _mapper.Map<Country>(countryDto);
            await _countryService.CreateAsync(country);
            return CreatedAtRoute("GetCountryById", new { id = country.Id }, countryDto);
        }

        [Authorize(Policy = "AdminOnly")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCountry(int id, [FromBody] CountryDto countryDto)
        {
            if (countryDto == null || countryDto.Id != id)
            {
                return BadRequest("Country data is invalid.");
            }

            var country = _mapper.Map<Country>(countryDto);
            try
            {
                await _countryService.UpdateAsync(country);
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
            var country = await _countryService.GetByIdAsync(id);
            if (country == null)
            {
                return NotFound();
            }

            await _countryService.DeleteAsync(country);
            return NoContent();
        }
    }
}