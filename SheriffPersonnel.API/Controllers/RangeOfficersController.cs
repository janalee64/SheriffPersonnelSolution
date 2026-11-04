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
    public class RangeOfficersController : ControllerBase
    {
        private readonly ICodeTableService _codeTableService;
        private readonly ILogger<RangeOfficersController> _logger;

        public RangeOfficersController(
            ICodeTableService codeTableService,
            ILogger<RangeOfficersController> logger)
        {
            _codeTableService = codeTableService ?? throw new ArgumentNullException(nameof(codeTableService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<RangeOfficerDTO>>> GetRangeOfficers()
        {
            try
            {
                var list = await _codeTableService.GetRangeOfficersAsync();
                return Ok(list);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching range officers.");
                return StatusCode(500, $"Error fetching range officers: {ex.Message}");
            }
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<RangeOfficerDTO>> GetRangeOfficer(int id)
        {
            try
            {
                var item = await _codeTableService.GetRangeOfficerByIdAsync(id);
                if (item == null) return NotFound($"Range officer with ID {id} not found.");
                return Ok(item);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching range officer {ID}.", id);
                return StatusCode(500, $"Error fetching range officer: {ex.Message}");
            }
        }

        [HttpPost]
        public async Task<ActionResult<RangeOfficerDTO>> CreateRangeOfficer([FromBody] RangeOfficerDTO dto)
        {
            if (dto == null) return BadRequest("Payload cannot be null.");
            try
            {
                var created = await _codeTableService.CreateRangeOfficerAsync(dto);
                return CreatedAtAction(nameof(GetRangeOfficer), new { id = created.RangeOfficerID }, created);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating range officer.");
                return StatusCode(500, $"Error creating range officer: {ex.Message}");
            }
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<RangeOfficerDTO>> UpdateRangeOfficer(int id, [FromBody] RangeOfficerDTO dto)
        {
            if (dto == null) return BadRequest("Payload cannot be null.");
            try
            {
                var updated = await _codeTableService.UpdateRangeOfficerAsync(id, dto);
                if (updated == null) return NotFound($"Range officer with ID {id} not found.");
                return Ok(updated);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating range officer {ID}.", id);
                return StatusCode(500, $"Error updating range officer: {ex.Message}");
            }
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteRangeOfficer(int id)
        {
            try
            {
                var success = await _codeTableService.DeleteRangeOfficerAsync(id);
                if (!success) return NotFound($"Range officer with ID {id} not found.");
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting range officer {ID}.", id);
                return StatusCode(500, $"Error deleting range officer: {ex.Message}");
            }
        }
    }
}

