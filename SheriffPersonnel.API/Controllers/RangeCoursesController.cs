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
    public class RangeCoursesController : ControllerBase
    {
        private readonly ICodeTableService _codeTableService;
        private readonly ILogger<RangeCoursesController> _logger;

        public RangeCoursesController(
            ICodeTableService codeTableService,
            ILogger<RangeCoursesController> logger)
        {
            _codeTableService = codeTableService ?? throw new ArgumentNullException(nameof(codeTableService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<RangeCourseDTO>>> GetRangeCourses()
        {
            try
            {
                var list = await _codeTableService.GetRangeCoursesAsync();
                return Ok(list);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching range courses.");
                return StatusCode(500, $"Error fetching range courses: {ex.Message}");
            }
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<RangeCourseDTO>> GetRangeCourse(int id)
        {
            try
            {
                var item = await _codeTableService.GetRangeCourseByIdAsync(id);
                if (item == null) return NotFound($"Range course with ID {id} not found.");
                return Ok(item);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching range course {ID}.", id);
                return StatusCode(500, $"Error fetching range course: {ex.Message}");
            }
        }

        [HttpPost]
        public async Task<ActionResult<RangeCourseDTO>> CreateRangeCourse([FromBody] RangeCourseDTO dto)
        {
            if (dto == null) return BadRequest("Payload cannot be null.");
            if (!ModelState.IsValid) return BadRequest(ModelState);
            try
            {
                var created = await _codeTableService.CreateRangeCourseAsync(dto);
                return CreatedAtAction(nameof(GetRangeCourse), new { id = created.CourseID }, created);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating range course.");
                return StatusCode(500, $"Error creating range course: {ex.Message}");
            }
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<RangeCourseDTO>> UpdateRangeCourse(int id, [FromBody] RangeCourseDTO dto)
        {
            if (dto == null) return BadRequest("Payload cannot be null.");
            if (!ModelState.IsValid) return BadRequest(ModelState);
            try
            {
                var updated = await _codeTableService.UpdateRangeCourseAsync(id, dto);
                if (updated == null) return NotFound($"Range course with ID {id} not found.");
                return Ok(updated);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating range course {ID}.", id);
                return StatusCode(500, $"Error updating range course: {ex.Message}");
            }
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteRangeCourse(int id)
        {
            try
            {
                var success = await _codeTableService.DeleteRangeCourseAsync(id);
                if (!success) return NotFound($"Range course with ID {id} not found.");
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting range course {ID}.", id);
                return StatusCode(500, $"Error deleting range course: {ex.Message}");
            }
        }
    }
}

