using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SheriffPersonnel.API.Services;
using SheriffPersonnel.Shared.DTOs;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace SheriffPersonnel.API.Controllers
{
    [ApiController]
    [Route("api/employee/search")]
    [Authorize(Roles = "Administrator, Personnel View, Personnel Edit, Inventory, Range Statistics, Range Scores, Training, Equipment, SSN")]
    public class EmployeeSearchController : ControllerBase
    {
        private readonly IEmployeeSearchService _searchService;
        private readonly Microsoft.Extensions.Logging.ILogger<EmployeeSearchController> _logger;

        public EmployeeSearchController(
            IEmployeeSearchService searchService,
            Microsoft.Extensions.Logging.ILogger<EmployeeSearchController> logger)
        {
            _searchService = searchService ?? throw new ArgumentNullException(nameof(searchService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpPost]
        public async Task<ActionResult<IEnumerable<EmployeeSearchResultDto>>> Search([FromBody] EmployeeSearchRequestDto request)
        {
            if (request == null)
            {
                _logger.LogWarning("Search endpoint called with null request body.");
                return BadRequest("Search request body cannot be null.");
            }

            try
            {
                _logger.LogInformation("Processing employee search request. FirstName: {FirstName}, LastName: {LastName}, Status: {Status}, Badge: {Badge}",
                    request.FirstName, request.LastName, request.Status, request.BadgeNumber);

                var results = await _searchService.SearchEmployeesAsync(request);
                return Ok(results);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while processing employee search.");
                return StatusCode(500, "An error occurred while searching for employees.");
            }
        }

        [HttpGet("statuses")]
        public async Task<ActionResult<IEnumerable<StatusesDto>>> GetStatuses()
        {
            try
            {
                _logger.LogInformation("Fetching available employee search statuses.");
                var statuses = await _searchService.GetStatusesAsync();
                return Ok(statuses);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving available employee statuses.");
                return StatusCode(500, "An error occurred while loading employee statuses.");
            }
        }

        [HttpGet("active")]
        public async Task<ActionResult<IEnumerable<EmployeeSearchResultDto>>> GetActive()
        {
            try
            {
                _logger.LogInformation("Fetching active employees.");
                var activeEmployees = await _searchService.GetActiveEmployeesAsync();
                return Ok(activeEmployees);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving active employees.");
                return StatusCode(500, "An error occurred while loading active employees.");
            }
        }
    }
}
