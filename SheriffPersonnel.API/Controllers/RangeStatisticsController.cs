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
    public class RangeStatisticsController : ControllerBase
    {
        private readonly IRangeStatisticService _rangeStatisticService;
        private readonly ILogger<RangeStatisticsController> _logger;

        public RangeStatisticsController(
            IRangeStatisticService rangeStatisticService,
            ILogger<RangeStatisticsController> logger)
        {
            _rangeStatisticService = rangeStatisticService ?? throw new ArgumentNullException(nameof(rangeStatisticService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<RangeStatisticDTO>>> GetAllRangeStatistics(
            [FromQuery] int? courseId = null,
            [FromQuery] int? employeeStatusId = null,
            [FromQuery] int? programId = null,
            [FromQuery] List<int>? unitIds = null,
            [FromQuery] string? startDate = null,
            [FromQuery] string? endDate = null,
            [FromQuery] string? passFail = null)
        {
            try
            {
                DateOnly? parsedStartDate = null;
                if (!string.IsNullOrWhiteSpace(startDate) && DateTime.TryParse(startDate, out var sd))
                {
                    parsedStartDate = DateOnly.FromDateTime(sd);
                }

                DateOnly? parsedEndDate = null;
                if (!string.IsNullOrWhiteSpace(endDate) && DateTime.TryParse(endDate, out var ed))
                {
                    parsedEndDate = DateOnly.FromDateTime(ed);
                }

                _logger.LogInformation("Retrieving range statistics with filters: courseId={courseId}, statusId={employeeStatusId}, programId={programId}, startDate={startDate}, endDate={endDate}, passFail={passFail}",
                    courseId, employeeStatusId, programId, startDate, endDate, passFail);

                var stats = await _rangeStatisticService.GetRangeStatisticsFilteredAsync(
                    courseId, employeeStatusId, programId, unitIds, parsedStartDate, parsedEndDate, passFail);

                return Ok(stats);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving range statistics.");
                return StatusCode(500, $"An error occurred while fetching range statistics: {ex.Message}");
            }
        }

        [HttpGet("employee/{employeeId:int}")]
        public async Task<ActionResult<IEnumerable<RangeStatisticDTO>>> GetRangeStatisticsByEmployeeId(int employeeId)
        {
            try
            {
                _logger.LogInformation("Retrieving range statistics for Employee ID {EmployeeID}.", employeeId);
                var stats = await _rangeStatisticService.GetRangeStatisticsByEmployeeIdAsync(employeeId);
                return Ok(stats);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving range statistics for Employee ID {EmployeeID}.", employeeId);
                return StatusCode(500, $"An error occurred while fetching range statistics: {ex.Message}");
            }
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<RangeStatisticDTO>> GetRangeStatisticById(int id)
        {
            try
            {
                _logger.LogInformation("Retrieving range statistic ID {RangeID}.", id);
                var stat = await _rangeStatisticService.GetRangeStatisticByIdAsync(id);
                if (stat == null) return NotFound($"Range statistic with ID {id} was not found.");

                return Ok(stat);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving range statistic ID {RangeID}.", id);
                return StatusCode(500, $"An error occurred while fetching range statistic: {ex.Message}");
            }
        }

        [HttpPost]
        [Authorize(Policy = "CanEditPersonnel")]
        public async Task<ActionResult<RangeStatisticDTO>> CreateRangeStatistic([FromBody] RangeStatisticDTO dto)
        {
            if (dto == null) return BadRequest("Payload cannot be null.");

            try
            {
                _logger.LogInformation("Creating new range statistic record for Employee ID {EmployeeID}.", dto.EmployeeID);
                var created = await _rangeStatisticService.CreateRangeStatisticAsync(dto);
                return CreatedAtAction(nameof(GetRangeStatisticById), new { id = created.RangeID }, created);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating range statistic record.");
                return StatusCode(500, $"An error occurred while creating range statistic: {ex.Message}");
            }
        }

        [HttpPut("{id:int}")]
        [Authorize(Policy = "CanEditPersonnel")]
        public async Task<ActionResult<RangeStatisticDTO>> UpdateRangeStatistic(int id, [FromBody] RangeStatisticDTO dto)
        {
            if (dto == null) return BadRequest("Payload cannot be null.");

            try
            {
                _logger.LogInformation("Updating range statistic ID {RangeID}.", id);
                var updated = await _rangeStatisticService.UpdateRangeStatisticAsync(id, dto);
                if (updated == null) return NotFound($"Range statistic with ID {id} was not found.");

                return Ok(updated);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating range statistic ID {RangeID}.", id);
                return StatusCode(500, $"An error occurred while updating range statistic: {ex.Message}");
            }
        }

        [HttpDelete("{id:int}")]
        [Authorize(Policy = "CanEditPersonnel")]
        public async Task<IActionResult> DeleteRangeStatistic(int id)
        {
            try
            {
                _logger.LogInformation("Deleting range statistic ID {RangeID}.", id);
                var success = await _rangeStatisticService.DeleteRangeStatisticAsync(id);
                if (!success) return NotFound($"Range statistic with ID {id} was not found.");

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting range statistic ID {RangeID}.", id);
                return StatusCode(500, $"An error occurred while deleting range statistic: {ex.Message}");
            }
        }

        [HttpGet("courses")]
        public async Task<ActionResult<IEnumerable<RangeCourseDTO>>> GetRangeCourses()
        {
            try
            {
                var courses = await _rangeStatisticService.GetRangeCoursesAsync();
                return Ok(courses);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving range courses.");
                return StatusCode(500, $"An error occurred while fetching range courses: {ex.Message}");
            }
        }

        [HttpGet("events")]
        public async Task<ActionResult<IEnumerable<RangeEventDTO>>> GetRangeEvents()
        {
            try
            {
                var events = await _rangeStatisticService.GetRangeEventsAsync();
                return Ok(events);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving range events.");
                return StatusCode(500, $"An error occurred while fetching range events: {ex.Message}");
            }
        }

        [HttpPost("events")]
        public async Task<ActionResult<RangeEventDTO>> CreateRangeEvent([FromBody] RangeEventDTO dto)
        {
            try
            {
                if (dto == null) return BadRequest("Range event payload is required.");
                var created = await _rangeStatisticService.CreateRangeEventAsync(dto);
                return CreatedAtAction(nameof(GetRangeEvents), new { id = created.RangeEventID }, created);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating range event.");
                return StatusCode(500, $"An error occurred while creating range event: {ex.Message}");
            }
        }
    }
}

