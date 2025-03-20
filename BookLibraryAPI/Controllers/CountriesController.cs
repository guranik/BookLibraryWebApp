using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BookLibraryAPI.Interfaces;
using BookLibraryAPI.Models;
using BookLibraryAPI.DTOs.Countries; // Assume you have a DTO for Country
using AutoMapper;
using System.Collections.Generic;

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

        [HttpGet]
        public ActionResult<IEnumerable<CountryDto>> GetAllCountries()
        {
            var countries = _countryService.AllCountries;
            var countryDtos = _mapper.Map<List<CountryDto>>(countries);
            return Ok(countryDtos);
        }

        [HttpGet("{id}", Name = "GetCountryById")]
        public ActionResult<CountryDto> GetCountryById(int id)
        {
            var country = _countryService.GetById(id);
            if (country == null)
            {
                return NotFound();
            }
            var countryDto = _mapper.Map<CountryDto>(country);
            return Ok(countryDto);
        }

        [Authorize(Policy = "AdminOnly")]
        [HttpPost]
        public ActionResult<CountryDto> CreateCountry([FromBody] CountryDto countryDto)
        {
            if (countryDto == null)
            {
                return BadRequest("Country cannot be null.");
            }

            var country = _mapper.Map<Country>(countryDto);
            _countryService.Create(country);
            return CreatedAtRoute("GetCountryById", new { id = country.Id }, countryDto);
        }

        [Authorize(Policy = "AdminOnly")]
        [HttpPut("{id}")]
        public IActionResult UpdateCountry(int id, [FromBody] CountryDto countryDto)
        {
            if (countryDto == null || countryDto.Id != id)
            {
                return BadRequest("Country data is invalid.");
            }

            var country = _mapper.Map<Country>(countryDto);
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