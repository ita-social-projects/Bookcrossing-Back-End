using System.Collections.Generic;
using System.Threading.Tasks;
using Application.Dto;
using Application.Dto.QueryParams;
using Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BookCrossingBackEnd.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LocationHomeController : ControllerBase
    {
        private readonly ILocationHomeService _locationService;
        private readonly IUserService _userService;

        public LocationHomeController(ILocationHomeService locationService, IUserService userService)
        {
            _locationService = locationService;
            _userService = userService;
        }

        // GET: api/Locations/5
        [HttpGet("{id}")]
        public async Task<ActionResult<LocationHomeDto>> GetLocation([FromRoute] int id)
        {
            var location = await _locationService.GetById(id);
            if (location == null)
                return NotFound();
            return Ok(location);
        }

        // GET: api/HomeLocations
        [HttpGet]
        public async Task<ActionResult<List<LocationHomeDto>>> GetAllLocations()
        {
            return Ok(await _locationService.GetAll());
        }

        // PUT: api/HomeLocations
        [HttpPut]
        public async Task<IActionResult> PutLocation([FromBody] LocationHomeDto locationDto)
        {
            await _locationService.Update(locationDto);
            return NoContent();
        }

        // POST: api/HomeLocations
        [HttpPost]
        public async Task<ActionResult<LocationHomeDto>> PostLocation([FromBody] LocationHomeDto locationDto)
        {
            var insertedId = await _locationService.Add(locationDto);
            locationDto.Id = insertedId;
            return CreatedAtAction("GetLocation", new { id = locationDto.Id }, locationDto);
        }

        // DELETE: api/Locations/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<LocationHomeDto>> DeleteLocation([FromRoute] int id)
        {
            var location = await _locationService.Remove(id);
            if (location == null)
                return NotFound();
            return Ok(location);
        }
        [HttpGet("paginated")]
        public async Task<ActionResult<PaginationDto<LocationHomeDto>>> GetAllGenres([FromQuery] FullPaginationQueryParams fullPaginationQuery)
        {
            return Ok(await _locationService.GetAll(fullPaginationQuery));
        }

    }
}
