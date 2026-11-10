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
    [Authorize(Roles = "Administrator, Personnel View, Personnel Edit, Inventory, Range Statistics, Range Scores, Training, Equipment, SSN")]
    public class UnitsController : ControllerBase
    {
        private readonly ICodeTableService _codeTableService;
        private readonly ILogger<UnitsController> _logger;

        public UnitsController(
            ICodeTableService codeTableService,
            ILogger<UnitsController> logger)
        {
            _codeTableService = codeTableService ?? throw new ArgumentNullException(nameof(codeTableService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<UnitsDto>>> GetUnits()
        {
            try
            {
                var list = await _codeTableService.GetUnitsAsync();
                return Ok(list);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching units.");
                return StatusCode(500, $"Error fetching units: {ex.Message}");
            }
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<UnitsDto>> GetUnit(int id)
        {
            try
            {
                var item = await _codeTableService.GetUnitByIdAsync(id);
                if (item == null) return NotFound($"Unit with ID {id} not found.");
                return Ok(item);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching unit {ID}.", id);
                return StatusCode(500, $"Error fetching unit: {ex.Message}");
            }
        }

        [HttpPost]
        public async Task<ActionResult<UnitsDto>> CreateUnit([FromBody] UnitsDto dto)
        {
            if (dto == null) return BadRequest("Payload cannot be null.");
            try
            {
                var created = await _codeTableService.CreateUnitAsync(dto);
                return CreatedAtAction(nameof(GetUnit), new { id = created.UnitID }, created);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating unit.");
                return StatusCode(500, $"Error creating unit: {ex.Message}");
            }
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<UnitsDto>> UpdateUnit(int id, [FromBody] UnitsDto dto)
        {
            if (dto == null) return BadRequest("Payload cannot be null.");
            try
            {
                var updated = await _codeTableService.UpdateUnitAsync(id, dto);
                if (updated == null) return NotFound($"Unit with ID {id} not found.");
                return Ok(updated);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating unit {ID}.", id);
                return StatusCode(500, $"Error updating unit: {ex.Message}");
            }
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteUnit(int id)
        {
            try
            {
                var success = await _codeTableService.DeleteUnitAsync(id);
                if (!success) return NotFound($"Unit with ID {id} not found.");
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting unit {ID}.", id);
                return StatusCode(500, $"Error deleting unit: {ex.Message}");
            }
        }
    }
}

