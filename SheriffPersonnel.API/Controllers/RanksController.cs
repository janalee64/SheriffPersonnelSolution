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
    public class RanksController : ControllerBase
    {
        private readonly ICodeTableService _codeTableService;
        private readonly ILogger<RanksController> _logger;

        public RanksController(
            ICodeTableService codeTableService,
            ILogger<RanksController> logger)
        {
            _codeTableService = codeTableService ?? throw new ArgumentNullException(nameof(codeTableService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<RanksDto>>> GetRanks()
        {
            try
            {
                var list = await _codeTableService.GetRanksAsync();
                return Ok(list);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching ranks.");
                return StatusCode(500, $"Error fetching ranks: {ex.Message}");
            }
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<RanksDto>> GetRank(int id)
        {
            try
            {
                var item = await _codeTableService.GetRankByIdAsync(id);
                if (item == null) return NotFound($"Rank with ID {id} not found.");
                return Ok(item);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching rank {ID}.", id);
                return StatusCode(500, $"Error fetching rank: {ex.Message}");
            }
        }

        [HttpPost]
        public async Task<ActionResult<RanksDto>> CreateRank([FromBody] RanksDto dto)
        {
            if (dto == null) return BadRequest("Payload cannot be null.");
            try
            {
                var created = await _codeTableService.CreateRankAsync(dto);
                return CreatedAtAction(nameof(GetRank), new { id = created.RankID }, created);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating rank.");
                return StatusCode(500, $"Error creating rank: {ex.Message}");
            }
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<RanksDto>> UpdateRank(int id, [FromBody] RanksDto dto)
        {
            if (dto == null) return BadRequest("Payload cannot be null.");
            try
            {
                var updated = await _codeTableService.UpdateRankAsync(id, dto);
                if (updated == null) return NotFound($"Rank with ID {id} not found.");
                return Ok(updated);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating rank {ID}.", id);
                return StatusCode(500, $"Error updating rank: {ex.Message}");
            }
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteRank(int id)
        {
            try
            {
                var success = await _codeTableService.DeleteRankAsync(id);
                if (!success) return NotFound($"Rank with ID {id} not found.");
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting rank {ID}.", id);
                return StatusCode(500, $"Error deleting rank: {ex.Message}");
            }
        }
    }
}

