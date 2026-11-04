using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SheriffPersonnel.API.Services;
using SheriffPersonnel.Shared.DTOs;

namespace SheriffPersonnel.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Administrator, Personnel, Inventory, Range, Training, Equipment, SSN")]
    public class LocationsController : ControllerBase
    {
        private readonly ICodeTableService _codeTableService;
        private readonly ILogger<LocationsController> _logger;

        public LocationsController(
            ICodeTableService codeTableService,
            ILogger<LocationsController> logger)
        {
            _codeTableService = codeTableService ?? throw new ArgumentNullException(nameof(codeTableService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<LocationsDto>>> GetLocations()
        {
            try
            {
                var list = await _codeTableService.GetLocationsAsync();
                return Ok(list);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching locations.");
                return StatusCode(500, $"Error fetching locations: {ex.Message}");
            }
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<LocationsDto>> GetLocation(int id)
        {
            try
            {
                var item = await _codeTableService.GetLocationByIdAsync(id);
                if (item == null) return NotFound($"Location with ID {id} not found.");
                return Ok(item);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching location {ID}.", id);
                return StatusCode(500, $"Error fetching location: {ex.Message}");
            }
        }

        [HttpPost]
        public async Task<ActionResult<LocationsDto>> CreateLocation([FromBody] LocationsDto dto)
        {
            if (dto == null) return BadRequest("Payload cannot be null.");
            try
            {
                var created = await _codeTableService.CreateLocationAsync(dto);
                return CreatedAtAction(nameof(GetLocation), new { id = created.LocationID }, created);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating location.");
                return StatusCode(500, $"Error creating location: {ex.Message}");
            }
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<LocationsDto>> UpdateLocation(int id, [FromBody] LocationsDto dto)
        {
            if (dto == null) return BadRequest("Payload cannot be null.");
            try
            {
                var updated = await _codeTableService.UpdateLocationAsync(id, dto);
                if (updated == null) return NotFound($"Location with ID {id} not found.");
                return Ok(updated);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating location {ID}.", id);
                return StatusCode(500, $"Error updating location: {ex.Message}");
            }
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteLocation(int id)
        {
            try
            {
                var success = await _codeTableService.DeleteLocationAsync(id);
                if (!success) return NotFound($"Location with ID {id} not found.");
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting location {ID}.", id);
                return StatusCode(500, $"Error deleting location: {ex.Message}");
            }
        }
    }
}

