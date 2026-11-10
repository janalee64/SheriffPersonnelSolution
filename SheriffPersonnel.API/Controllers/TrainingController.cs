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
    public class TrainingController : ControllerBase
    {
        private readonly ITrainingService _trainingService;
        private readonly ILogger<TrainingController> _logger;

        public TrainingController(
            ITrainingService trainingService,
            ILogger<TrainingController> logger)
        {
            _trainingService = trainingService ?? throw new ArgumentNullException(nameof(trainingService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpGet("employee/{employeeId:int}")]
        public async Task<ActionResult<IEnumerable<EmployeeTrainingsDto>>> GetEmployeeTrainingsByEmployeeId(int employeeId)
        {
            try
            {
                _logger.LogInformation("Retrieving training records for Employee ID {EmployeeID}.", employeeId);
                var trainings = await _trainingService.GetEmployeeTrainingsByEmployeeIdAsync(employeeId);
                return Ok(trainings);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving training records for Employee ID {EmployeeID}.", employeeId);
                return StatusCode(500, $"An error occurred while fetching training records: {ex.Message}");
            }
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<EmployeeTrainingsDto>> GetEmployeeTrainingById(int id)
        {
            try
            {
                _logger.LogInformation("Retrieving employee training ID {EmployeeTrainingID}.", id);
                var training = await _trainingService.GetEmployeeTrainingByIdAsync(id);
                if (training == null) return NotFound($"Employee training record with ID {id} was not found.");

                return Ok(training);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving employee training ID {EmployeeTrainingID}.", id);
                return StatusCode(500, $"An error occurred while fetching training record: {ex.Message}");
            }
        }

        [HttpPost]
        [Authorize(Policy = "CanEditPersonnel")]
        public async Task<ActionResult<EmployeeTrainingsDto>> CreateEmployeeTraining([FromBody] EmployeeTrainingsDto dto)
        {
            if (dto == null) return BadRequest("Payload cannot be null.");

            try
            {
                _logger.LogInformation("Creating new employee training record for Employee ID {EmployeeID}.", dto.EmployeeID);
                var created = await _trainingService.CreateEmployeeTrainingAsync(dto);
                return CreatedAtAction(nameof(GetEmployeeTrainingById), new { id = created.EmployeeTrainingID }, created);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating employee training record.");
                return StatusCode(500, $"An error occurred while creating training record: {ex.Message}");
            }
        }

        [HttpPut("{id:int}")]
        [Authorize(Policy = "CanEditPersonnel")]
        public async Task<ActionResult<EmployeeTrainingsDto>> UpdateEmployeeTraining(int id, [FromBody] EmployeeTrainingsDto dto)
        {
            if (dto == null) return BadRequest("Payload cannot be null.");

            try
            {
                _logger.LogInformation("Updating employee training record ID {EmployeeTrainingID}.", id);
                var updated = await _trainingService.UpdateEmployeeTrainingAsync(id, dto);
                if (updated == null) return NotFound($"Employee training record with ID {id} was not found.");

                return Ok(updated);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating employee training record ID {EmployeeTrainingID}.", id);
                return StatusCode(500, $"An error occurred while updating training record: {ex.Message}");
            }
        }

        [HttpDelete("{id:int}")]
        [Authorize(Policy = "CanEditPersonnel")]
        public async Task<IActionResult> DeleteEmployeeTraining(int id)
        {
            try
            {
                _logger.LogInformation("Deleting employee training record ID {EmployeeTrainingID}.", id);
                var success = await _trainingService.DeleteEmployeeTrainingAsync(id);
                if (!success) return NotFound($"Employee training record with ID {id} was not found.");

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting employee training record ID {EmployeeTrainingID}.", id);
                return StatusCode(500, $"An error occurred while deleting training record: {ex.Message}");
            }
        }

        [HttpGet("expirations")]
        public async Task<ActionResult<IEnumerable<TrainingExpirationDto>>> GetTrainingExpirations(
            [FromQuery] DateOnly? startDate,
            [FromQuery] DateOnly? endDate)
        {
            try
            {
                DateOnly start = startDate ?? DateOnly.FromDateTime(DateTime.Today);
                DateOnly end = endDate ?? DateOnly.FromDateTime(DateTime.Today.AddDays(60));

                _logger.LogInformation("Retrieving training expirations between {StartDate} and {EndDate}.", start, end);
                var expirations = await _trainingService.GetTrainingExpirationsAsync(start, end);
                return Ok(expirations);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving training expirations.");
                return StatusCode(500, $"An error occurred while fetching training expirations: {ex.Message}");
            }
        }

        [HttpPost("by-school")]
        public async Task<ActionResult<IEnumerable<TrainingBySchoolDto>>> GetTrainingBySchool([FromBody] TrainingBySchoolRequestDto request)
        {
            if (request == null) return BadRequest("Payload cannot be null.");

            try
            {
                _logger.LogInformation("Retrieving trainings by school for SchoolName: '{SchoolName}', SchoolID: {SchoolID}.", request.SchoolName, request.TrainingSchoolID);
                var results = await _trainingService.GetTrainingBySchoolAsync(request);
                return Ok(results);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving trainings by school.");
                return StatusCode(500, $"An error occurred while fetching training by school records: {ex.Message}");
            }
        }

        [HttpGet("sessions")]
        public async Task<ActionResult<IEnumerable<TrainingSessionsDto>>> GetSessions()
        {
            try
            {
                var sessions = await _trainingService.GetTrainingSessionsAsync();
                return Ok(sessions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving training sessions.");
                return StatusCode(500, $"An error occurred while fetching training sessions: {ex.Message}");
            }
        }

        [HttpPost("sessions")]
        [Authorize(Policy = "CanEditPersonnel")]
        public async Task<ActionResult<TrainingSessionsDto>> CreateTrainingSession([FromBody] TrainingSessionsDto dto)
        {
            if (dto == null) return BadRequest("Payload cannot be null.");

            try
            {
                _logger.LogInformation("Creating new training session.");
                var created = await _trainingService.CreateTrainingSessionAsync(dto);
                return Ok(created);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating training session.");
                return StatusCode(500, $"An error occurred while creating training session: {ex.Message}");
            }
        }

        [HttpPut("sessions/{id:int}")]
        [Authorize(Policy = "CanEditPersonnel")]
        public async Task<ActionResult<TrainingSessionsDto>> UpdateTrainingSession(int id, [FromBody] TrainingSessionsDto dto)
        {
            if (dto == null) return BadRequest("Payload cannot be null.");

            try
            {
                _logger.LogInformation("Updating training session ID {SessionID}.", id);
                var updated = await _trainingService.UpdateTrainingSessionAsync(id, dto);
                if (updated == null) return NotFound($"Training session with ID {id} was not found.");

                return Ok(updated);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating training session ID {SessionID}.", id);
                return StatusCode(500, $"An error occurred while updating training session: {ex.Message}");
            }
        }

        [HttpGet("schools")]
        public async Task<ActionResult<IEnumerable<TrainingSchoolsDto>>> GetSchools()
        {
            try
            {
                var schools = await _trainingService.GetTrainingSchoolsAsync();
                return Ok(schools);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving training schools.");
                return StatusCode(500, $"An error occurred while fetching training schools: {ex.Message}");
            }
        }

        [HttpPost("assign-multiple")]
        [Authorize(Policy = "CanEditPersonnel")]
        public async Task<ActionResult<MultipleTrainingAssignmentResultDto>> AssignMultipleTrainings([FromBody] MultipleTrainingAssignmentRequestDto request)
        {
            if (request == null) return BadRequest("Payload cannot be null.");

            try
            {
                _logger.LogInformation("Assigning training session to {Count} employees.", request.EmployeeIDs?.Count ?? 0);
                var result = await _trainingService.AssignMultipleTrainingsAsync(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error assigning multiple trainings.");
                return StatusCode(500, $"An error occurred while assigning training session: {ex.Message}");
            }
        }
    }
}
