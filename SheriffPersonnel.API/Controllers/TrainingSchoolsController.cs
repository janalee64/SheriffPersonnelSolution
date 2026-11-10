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
    public class TrainingSchoolsController : ControllerBase
    {
        private readonly ICodeTableService _codeTableService;
        private readonly ILogger<TrainingSchoolsController> _logger;

        public TrainingSchoolsController(
            ICodeTableService codeTableService,
            ILogger<TrainingSchoolsController> logger)
        {
            _codeTableService = codeTableService ?? throw new ArgumentNullException(nameof(codeTableService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TrainingSchoolsDto>>> GetTrainingSchools()
        {
            try
            {
                var list = await _codeTableService.GetTrainingSchoolsAsync();
                return Ok(list);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching training schools.");
                return StatusCode(500, $"Error fetching training schools: {ex.Message}");
            }
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<TrainingSchoolsDto>> GetTrainingSchool(int id)
        {
            try
            {
                var item = await _codeTableService.GetTrainingSchoolByIdAsync(id);
                if (item == null) return NotFound($"Training school with ID {id} not found.");
                return Ok(item);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching training school {ID}.", id);
                return StatusCode(500, $"Error fetching training school: {ex.Message}");
            }
        }

        [HttpPost]
        public async Task<ActionResult<TrainingSchoolsDto>> CreateTrainingSchool([FromBody] TrainingSchoolsDto dto)
        {
            if (dto == null) return BadRequest("Payload cannot be null.");
            try
            {
                var created = await _codeTableService.CreateTrainingSchoolAsync(dto);
                return CreatedAtAction(nameof(GetTrainingSchool), new { id = created.TrainingSchoolID }, created);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating training school.");
                return StatusCode(500, $"Error creating training school: {ex.Message}");
            }
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<TrainingSchoolsDto>> UpdateTrainingSchool(int id, [FromBody] TrainingSchoolsDto dto)
        {
            if (dto == null) return BadRequest("Payload cannot be null.");
            try
            {
                var updated = await _codeTableService.UpdateTrainingSchoolAsync(id, dto);
                if (updated == null) return NotFound($"Training school with ID {id} not found.");
                return Ok(updated);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating training school {ID}.", id);
                return StatusCode(500, $"Error updating training school: {ex.Message}");
            }
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteTrainingSchool(int id)
        {
            try
            {
                var success = await _codeTableService.DeleteTrainingSchoolAsync(id);
                if (!success) return NotFound($"Training school with ID {id} not found.");
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting training school {ID}.", id);
                return StatusCode(500, $"Error deleting training school: {ex.Message}");
            }
        }
    }
}

