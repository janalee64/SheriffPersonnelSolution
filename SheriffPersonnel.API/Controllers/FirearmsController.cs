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
    public class FirearmsController : ControllerBase
    {
        private readonly IFirearmService _firearmService;
        private readonly ILogger<FirearmsController> _logger;

        public FirearmsController(
            IFirearmService firearmService,
            ILogger<FirearmsController> logger)
        {
            _firearmService = firearmService ?? throw new ArgumentNullException(nameof(firearmService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<FirearmsDto>>> GetAllFirearms()
        {
            try
            {
                _logger.LogInformation("Retrieving all firearms.");
                var firearms = await _firearmService.GetAllFirearmsAsync();
                return Ok(firearms);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all firearms.");
                return StatusCode(500, $"An error occurred while fetching firearms: {ex.Message}");
            }
        }

        [HttpGet("employee/{employeeId:int}")]
        public async Task<ActionResult<IEnumerable<FirearmsDto>>> GetFirearmsByEmployeeId(int employeeId)
        {
            try
            {
                _logger.LogInformation("Retrieving firearms for Employee ID {EmployeeID}.", employeeId);
                var firearms = await _firearmService.GetFirearmsByEmployeeIdAsync(employeeId);
                return Ok(firearms);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving firearms for Employee ID {EmployeeID}.", employeeId);
                return StatusCode(500, $"An error occurred while fetching firearms: {ex.Message}");
            }
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<FirearmsDto>> GetFirearmById(int id)
        {
            try
            {
                _logger.LogInformation("Retrieving firearm ID {FirearmID}.", id);
                var firearm = await _firearmService.GetFirearmByIdAsync(id);
                if (firearm == null) return NotFound($"Firearm with ID {id} was not found.");

                return Ok(firearm);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving firearm ID {FirearmID}.", id);
                return StatusCode(500, $"An error occurred while fetching firearm: {ex.Message}");
            }
        }

        [HttpPost]
        public async Task<ActionResult<FirearmsDto>> CreateFirearm([FromBody] FirearmsDto dto)
        {
            if (dto == null) return BadRequest("Payload cannot be null.");

            try
            {
                _logger.LogInformation("Creating new firearm record for Employee ID {EmployeeID}.", dto.EmployeeID);
                var created = await _firearmService.CreateFirearmAsync(dto);
                return CreatedAtAction(nameof(GetFirearmById), new { id = created.FirearmID }, created);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating firearm record.");
                return StatusCode(500, $"An error occurred while creating firearm: {ex.Message}");
            }
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<FirearmsDto>> UpdateFirearm(int id, [FromBody] FirearmsDto dto)
        {
            if (dto == null) return BadRequest("Payload cannot be null.");

            try
            {
                _logger.LogInformation("Updating firearm ID {FirearmID}.", id);
                var updated = await _firearmService.UpdateFirearmAsync(id, dto);
                if (updated == null) return NotFound($"Firearm with ID {id} was not found.");

                return Ok(updated);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating firearm ID {FirearmID}.", id);
                return StatusCode(500, $"An error occurred while updating firearm: {ex.Message}");
            }
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteFirearm(int id)
        {
            try
            {
                _logger.LogInformation("Deleting firearm ID {FirearmID}.", id);
                var success = await _firearmService.DeleteFirearmAsync(id);
                if (!success) return NotFound($"Firearm with ID {id} was not found.");

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting firearm ID {FirearmID}.", id);
                return StatusCode(500, $"An error occurred while deleting firearm: {ex.Message}");
            }
        }

        [HttpGet("makes")]
        public async Task<ActionResult<IEnumerable<MakesDto>>> GetMakes()
        {
            try
            {
                var makes = await _firearmService.GetMakesAsync();
                return Ok(makes);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving firearm makes.");
                return StatusCode(500, $"An error occurred while fetching firearm makes: {ex.Message}");
            }
        }
    }
}

