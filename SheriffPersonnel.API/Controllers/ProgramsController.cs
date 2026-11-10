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
    public class ProgramsController : ControllerBase
    {
        private readonly ICodeTableService _codeTableService;
        private readonly ILogger<ProgramsController> _logger;

        public ProgramsController(
            ICodeTableService codeTableService,
            ILogger<ProgramsController> logger)
        {
            _codeTableService = codeTableService ?? throw new ArgumentNullException(nameof(codeTableService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProgramsDto>>> GetPrograms()
        {
            try
            {
                var list = await _codeTableService.GetProgramsAsync();
                return Ok(list);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching programs.");
                return StatusCode(500, $"Error fetching programs: {ex.Message}");
            }
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<ProgramsDto>> GetProgram(int id)
        {
            try
            {
                var item = await _codeTableService.GetProgramByIdAsync(id);
                if (item == null) return NotFound($"Program with ID {id} not found.");
                return Ok(item);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching program {ID}.", id);
                return StatusCode(500, $"Error fetching program: {ex.Message}");
            }
        }

        [HttpPost]
        public async Task<ActionResult<ProgramsDto>> CreateProgram([FromBody] ProgramsDto dto)
        {
            if (dto == null) return BadRequest("Payload cannot be null.");
            try
            {
                var created = await _codeTableService.CreateProgramAsync(dto);
                return CreatedAtAction(nameof(GetProgram), new { id = created.ProgramID }, created);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating program.");
                return StatusCode(500, $"Error creating program: {ex.Message}");
            }
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<ProgramsDto>> UpdateProgram(int id, [FromBody] ProgramsDto dto)
        {
            if (dto == null) return BadRequest("Payload cannot be null.");
            try
            {
                var updated = await _codeTableService.UpdateProgramAsync(id, dto);
                if (updated == null) return NotFound($"Program with ID {id} not found.");
                return Ok(updated);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating program {ID}.", id);
                return StatusCode(500, $"Error updating program: {ex.Message}");
            }
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteProgram(int id)
        {
            try
            {
                var success = await _codeTableService.DeleteProgramAsync(id);
                if (!success) return NotFound($"Program with ID {id} not found.");
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting program {ID}.", id);
                return StatusCode(500, $"Error deleting program: {ex.Message}");
            }
        }
    }
}

