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
    public class StatusesController : ControllerBase
    {
        private readonly ICodeTableService _codeTableService;
        private readonly ILogger<StatusesController> _logger;

        public StatusesController(
            ICodeTableService codeTableService,
            ILogger<StatusesController> logger)
        {
            _codeTableService = codeTableService ?? throw new ArgumentNullException(nameof(codeTableService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<StatusesDto>>> GetStatuses()
        {
            try
            {
                var list = await _codeTableService.GetStatusesAsync();
                return Ok(list);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching statuses.");
                return StatusCode(500, $"Error fetching statuses: {ex.Message}");
            }
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<StatusesDto>> GetStatus(int id)
        {
            try
            {
                var item = await _codeTableService.GetStatusByIdAsync(id);
                if (item == null) return NotFound($"Status with ID {id} not found.");
                return Ok(item);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching status {ID}.", id);
                return StatusCode(500, $"Error fetching status: {ex.Message}");
            }
        }

        [HttpPost]
        public async Task<ActionResult<StatusesDto>> CreateStatus([FromBody] StatusesDto dto)
        {
            if (dto == null) return BadRequest("Payload cannot be null.");
            try
            {
                var created = await _codeTableService.CreateStatusAsync(dto);
                return CreatedAtAction(nameof(GetStatus), new { id = created.StatusID }, created);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating status.");
                return StatusCode(500, $"Error creating status: {ex.Message}");
            }
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<StatusesDto>> UpdateStatus(int id, [FromBody] StatusesDto dto)
        {
            if (dto == null) return BadRequest("Payload cannot be null.");
            try
            {
                var updated = await _codeTableService.UpdateStatusAsync(id, dto);
                if (updated == null) return NotFound($"Status with ID {id} not found.");
                return Ok(updated);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating status {ID}.", id);
                return StatusCode(500, $"Error updating status: {ex.Message}");
            }
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteStatus(int id)
        {
            try
            {
                var success = await _codeTableService.DeleteStatusAsync(id);
                if (!success) return NotFound($"Status with ID {id} not found.");
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting status {ID}.", id);
                return StatusCode(500, $"Error deleting status: {ex.Message}");
            }
        }
    }
}

