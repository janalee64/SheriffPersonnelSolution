using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SheriffPersonnel.API.Services;
using SheriffPersonnel.Shared.DTOs;

namespace SheriffPersonnel.API.Controllers
{
    [ApiController]
    [Route("api/employee")]
    [Authorize(Roles = "Administrator, Personnel, Inventory, Range, Training, Equipment, SSN")]
    public class EmployeeController : ControllerBase
    {
        private readonly IEmployeeSearchService _employeeService;
        private readonly ILogger<EmployeeController> _logger;

        public EmployeeController(
            IEmployeeSearchService employeeService,
            ILogger<EmployeeController> logger)
        {
            _employeeService = employeeService ?? throw new ArgumentNullException(nameof(employeeService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<EmployeeDetailDto>> GetById(int id)
        {
            try
            {
                _logger.LogInformation("Retrieving employee detail for ID {EmployeeID}.", id);
                var employee = await _employeeService.GetEmployeeByIdAsync(id);

                if (employee == null)
                {
                    _logger.LogWarning("Employee with ID {EmployeeID} was not found.", id);
                    return NotFound($"Employee with ID {id} was not found.");
                }

                if (!User.IsInRole("SSN"))
                {
                    employee.SSN = null;
                }

                return Ok(employee);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving employee ID {EmployeeID}.", id);
                var detail = ex.InnerException != null ? $"{ex.Message} -> {ex.InnerException.Message}" : ex.Message;
                return StatusCode(500, $"An error occurred while fetching employee details: {detail}");
            }
        }

        [HttpPost]
        public async Task<ActionResult<EmployeeDetailDto>> Create([FromBody] EmployeeDetailDto dto)
        {
            if (dto == null)
            {
                return BadRequest("Employee payload cannot be null.");
            }

            try
            {
                if (!User.IsInRole("SSN"))
                {
                    dto.SSN = null;
                }

                _logger.LogInformation("Creating new employee record for {FirstName} {LastName}.", dto.FirstName, dto.LastName);
                var created = await _employeeService.CreateEmployeeAsync(dto);

                if (!User.IsInRole("SSN"))
                {
                    created.SSN = null;
                }

                return CreatedAtAction(nameof(GetById), new { id = created.EmployeeID }, created);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating new employee.");
                var detail = ex.InnerException != null ? $"{ex.Message} -> {ex.InnerException.Message}" : ex.Message;
                return StatusCode(500, $"An error occurred while creating the employee: {detail}");
            }
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<EmployeeDetailDto>> Update(int id, [FromBody] EmployeeDetailDto dto)
        {
            if (dto == null)
            {
                return BadRequest("Employee payload cannot be null.");
            }

            try
            {
                _logger.LogInformation("Updating employee record for ID {EmployeeID}.", id);

                if (!User.IsInRole("SSN"))
                {
                    var existing = await _employeeService.GetEmployeeByIdAsync(id);
                    if (existing != null)
                    {
                        dto.SSN = existing.SSN;
                    }
                }

                var updated = await _employeeService.UpdateEmployeeAsync(id, dto);

                if (updated == null)
                {
                    return NotFound($"Employee with ID {id} was not found.");
                }

                if (!User.IsInRole("SSN"))
                {
                    updated.SSN = null;
                }

                return Ok(updated);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating employee ID {EmployeeID}.", id);
                var detail = ex.InnerException != null ? $"{ex.Message} -> {ex.InnerException.Message}" : ex.Message;
                return StatusCode(500, $"An error occurred while updating the employee: {detail}");
            }
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                _logger.LogInformation("Deleting employee record for ID {EmployeeID}.", id);
                var success = await _employeeService.DeleteEmployeeAsync(id);

                if (!success)
                {
                    return NotFound($"Employee with ID {id} was not found.");
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting employee ID {EmployeeID}.", id);
                var detail = ex.InnerException != null ? $"{ex.Message} -> {ex.InnerException.Message}" : ex.Message;
                return StatusCode(500, $"An error occurred while deleting the employee: {detail}");
            }
        }
    }
}

