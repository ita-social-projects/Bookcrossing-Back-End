using System.Collections.Generic;
using System.Threading.Tasks;
using Application.Dto;
using Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BookCrossingBackEnd.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LocationHomeController : ControllerBase
    {
        private readonly ILocationHomeService _locationService;

        public LocationHomeController(ILocationHomeService locationService)
        {
            _locationService = locationService;
        }

        // GET: api/LocationsHome/5
        [HttpGet("{id}")]
        public async Task<ActionResult<LocationHomeDto>> GetLocation([FromRoute] int id)
        {
            var location = await _locationService.GetById(id);
            if (location == null)
                return NotFound();
            return Ok(location);
        }

        // GET: api/HomeLocation
        [HttpGet]
        public async Task<ActionResult<List<LocationHomeDto>>> GetAllLocations()
        {
            return Ok(await _locationService.GetAll());
        }

        // PUT: api/LocationHome
        [HttpPut]
        public async Task<IActionResult> PutLocation([FromBody] LocationHomeDto locationDto)
        {
            await _locationService.Update(locationDto);
            return NoContent();
        }

        // POST: api/LocationHome
        [HttpPost]
        public async Task<ActionResult<LocationHomeDto>> PostLocation([FromBody] LocationHomePostDto locationDto)
        {
            var insertedId = await _locationService.Add(locationDto);
            locationDto.Id = insertedId;
            return CreatedAtAction("GetLocation", new { id = locationDto.Id }, locationDto);
        }

        // DELETE: api/LocationHome/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<LocationHomeDto>> DeleteLocation([FromRoute] int id)
        {
            var location = await _locationService.Remove(id);
            if (location == null)
                return NotFound();
            return Ok(location);
        }
    }
}
