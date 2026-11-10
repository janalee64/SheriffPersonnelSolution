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
    public class RangeSkillsController : ControllerBase
    {
        private readonly ICodeTableService _codeTableService;
        private readonly ILogger<RangeSkillsController> _logger;

        public RangeSkillsController(
            ICodeTableService codeTableService,
            ILogger<RangeSkillsController> logger)
        {
            _codeTableService = codeTableService ?? throw new ArgumentNullException(nameof(codeTableService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<RangeSkillDTO>>> GetRangeSkills()
        {
            try
            {
                var list = await _codeTableService.GetRangeSkillsAsync();
                return Ok(list);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching range skills.");
                return StatusCode(500, $"Error fetching range skills: {ex.Message}");
            }
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<RangeSkillDTO>> GetRangeSkill(int id)
        {
            try
            {
                var item = await _codeTableService.GetRangeSkillByIdAsync(id);
                if (item == null) return NotFound($"Range skill with ID {id} not found.");
                return Ok(item);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching range skill {ID}.", id);
                return StatusCode(500, $"Error fetching range skill: {ex.Message}");
            }
        }

        [HttpPost]
        public async Task<ActionResult<RangeSkillDTO>> CreateRangeSkill([FromBody] RangeSkillDTO dto)
        {
            if (dto == null) return BadRequest("Payload cannot be null.");
            try
            {
                var created = await _codeTableService.CreateRangeSkillAsync(dto);
                return CreatedAtAction(nameof(GetRangeSkill), new { id = created.RangeSkillID }, created);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating range skill.");
                return StatusCode(500, $"Error creating range skill: {ex.Message}");
            }
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<RangeSkillDTO>> UpdateRangeSkill(int id, [FromBody] RangeSkillDTO dto)
        {
            if (dto == null) return BadRequest("Payload cannot be null.");
            try
            {
                var updated = await _codeTableService.UpdateRangeSkillAsync(id, dto);
                if (updated == null) return NotFound($"Range skill with ID {id} not found.");
                return Ok(updated);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating range skill {ID}.", id);
                return StatusCode(500, $"Error updating range skill: {ex.Message}");
            }
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteRangeSkill(int id)
        {
            try
            {
                var success = await _codeTableService.DeleteRangeSkillAsync(id);
                if (!success) return NotFound($"Range skill with ID {id} not found.");
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting range skill {ID}.", id);
                return StatusCode(500, $"Error deleting range skill: {ex.Message}");
            }
        }
    }
}

