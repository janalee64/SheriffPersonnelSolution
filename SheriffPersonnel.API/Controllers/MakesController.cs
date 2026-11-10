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
    public class MakesController : ControllerBase
    {
        private readonly ICodeTableService _codeTableService;
        private readonly ILogger<MakesController> _logger;

        public MakesController(
            ICodeTableService codeTableService,
            ILogger<MakesController> logger)
        {
            _codeTableService = codeTableService ?? throw new ArgumentNullException(nameof(codeTableService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<MakesDto>>> GetMakes()
        {
            try
            {
                var list = await _codeTableService.GetMakesAsync();
                return Ok(list);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching makes.");
                return StatusCode(500, $"Error fetching makes: {ex.Message}");
            }
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<MakesDto>> GetMake(int id)
        {
            try
            {
                var item = await _codeTableService.GetMakeByIdAsync(id);
                if (item == null) return NotFound($"Make with ID {id} not found.");
                return Ok(item);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching make {ID}.", id);
                return StatusCode(500, $"Error fetching make: {ex.Message}");
            }
        }

        [HttpPost]
        [Authorize(Roles = "Administrator")]
        public async Task<ActionResult<MakesDto>> CreateMake([FromBody] MakesDto dto)
        {
            if (dto == null) return BadRequest("Payload cannot be null.");
            try
            {
                var created = await _codeTableService.CreateMakeAsync(dto);
                return CreatedAtAction(nameof(GetMake), new { id = created.MakeID }, created);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating make.");
                return StatusCode(500, $"Error creating make: {ex.Message}");
            }
        }

        [HttpPut("{id:int}")]
        [Authorize(Roles = "Administrator")]
        public async Task<ActionResult<MakesDto>> UpdateMake(int id, [FromBody] MakesDto dto)
        {
            if (dto == null) return BadRequest("Payload cannot be null.");
            try
            {
                var updated = await _codeTableService.UpdateMakeAsync(id, dto);
                if (updated == null) return NotFound($"Make with ID {id} not found.");
                return Ok(updated);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating make {ID}.", id);
                return StatusCode(500, $"Error updating make: {ex.Message}");
            }
        }

        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> DeleteMake(int id)
        {
            try
            {
                var success = await _codeTableService.DeleteMakeAsync(id);
                if (!success) return NotFound($"Make with ID {id} not found.");
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting make {ID}.", id);
                return StatusCode(500, $"Error deleting make: {ex.Message}");
            }
        }
    }
}

