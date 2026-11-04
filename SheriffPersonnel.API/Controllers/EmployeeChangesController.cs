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
    public class EmployeeChangesController : ControllerBase
    {
        private readonly IEmployeeChangeService _employeeChangeService;
        private readonly ILogger<EmployeeChangesController> _logger;

        public EmployeeChangesController(
            IEmployeeChangeService employeeChangeService,
            ILogger<EmployeeChangesController> logger)
        {
            _employeeChangeService = employeeChangeService ?? throw new ArgumentNullException(nameof(employeeChangeService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpGet("employee/{employeeId:int}")]
        public async Task<ActionResult<IEnumerable<EmployeeChangesDto>>> GetByEmployeeId(int employeeId)
        {
            try
            {
                _logger.LogInformation("Retrieving employee changes for Employee ID {EmployeeID}.", employeeId);
                var changes = await _employeeChangeService.GetChangesByEmployeeIdAsync(employeeId);
                return Ok(changes);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving employee changes for Employee ID {EmployeeID}.", employeeId);
                return StatusCode(500, $"An error occurred while fetching employee changes: {ex.Message}");
            }
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<EmployeeChangesDto>> GetById(int id)
        {
            try
            {
                _logger.LogInformation("Retrieving employee change ID {HistoryID}.", id);
                var change = await _employeeChangeService.GetChangeByIdAsync(id);
                if (change == null) return NotFound($"Employee change with ID {id} was not found.");

                return Ok(change);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving employee change ID {HistoryID}.", id);
                return StatusCode(500, $"An error occurred while fetching employee change: {ex.Message}");
            }
        }

        [HttpPost]
        public async Task<ActionResult<EmployeeChangesDto>> Create([FromBody] EmployeeChangesDto dto)
        {
            if (dto == null) return BadRequest("Payload cannot be null.");

            try
            {
                _logger.LogInformation("Creating new employee change for Employee ID {EmployeeID}.", dto.EmployeeID);
                var created = await _employeeChangeService.CreateChangeAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = created.HistoryID }, created);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating employee change.");
                return StatusCode(500, $"An error occurred while creating employee change: {ex.Message}");
            }
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<EmployeeChangesDto>> Update(int id, [FromBody] EmployeeChangesDto dto)
        {
            if (dto == null) return BadRequest("Payload cannot be null.");

            try
            {
                _logger.LogInformation("Updating employee change ID {HistoryID}.", id);
                var updated = await _employeeChangeService.UpdateChangeAsync(id, dto);
                if (updated == null) return NotFound($"Employee change with ID {id} was not found.");

                return Ok(updated);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating employee change ID {HistoryID}.", id);
                return StatusCode(500, $"An error occurred while updating employee change: {ex.Message}");
            }
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                _logger.LogInformation("Deleting employee change ID {HistoryID}.", id);
                var success = await _employeeChangeService.DeleteChangeAsync(id);
                if (!success) return NotFound($"Employee change with ID {id} was not found.");

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting employee change ID {HistoryID}.", id);
                return StatusCode(500, $"An error occurred while deleting employee change: {ex.Message}");
            }
        }

        [HttpGet("programs")]
        public async Task<ActionResult<IEnumerable<ProgramsDto>>> GetPrograms()
        {
            try
            {
                var programs = await _employeeChangeService.GetProgramsAsync();
                return Ok(programs);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving programs.");
                return StatusCode(500, $"An error occurred while fetching programs: {ex.Message}");
            }
        }

        [HttpGet("ranks")]
        public async Task<ActionResult<IEnumerable<RanksDto>>> GetRanks()
        {
            try
            {
                var ranks = await _employeeChangeService.GetRanksAsync();
                return Ok(ranks);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving ranks.");
                return StatusCode(500, $"An error occurred while fetching ranks: {ex.Message}");
            }
        }

        [HttpGet("units")]
        public async Task<ActionResult<IEnumerable<UnitsDto>>> GetUnits()
        {
            try
            {
                var units = await _employeeChangeService.GetUnitsAsync();
                return Ok(units);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving units.");
                return StatusCode(500, $"An error occurred while fetching units: {ex.Message}");
            }
        }
    }
}

