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
    public class InventoryController : ControllerBase
    {
        private readonly IInventoryService _inventoryService;
        private readonly ILogger<InventoryController> _logger;

        public InventoryController(
            IInventoryService inventoryService,
            ILogger<InventoryController> logger)
        {
            _inventoryService = inventoryService ?? throw new ArgumentNullException(nameof(inventoryService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpGet("assignments")]
        public async Task<ActionResult<IEnumerable<EmployeeInventoryAssignmentsDto>>> GetAllAssignments(
            [FromQuery] string? productName = null,
            [FromQuery] int? programId = null,
            [FromQuery] int? statusId = null,
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null,
            [FromQuery] int? maxResults = null)
        {
            try
            {
                _logger.LogInformation("Retrieving inventory assignments with filters.");
                var assignments = await _inventoryService.GetAllAssignmentsAsync(productName, programId, statusId, startDate, endDate, maxResults);
                return Ok(assignments);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving inventory assignments.");
                return StatusCode(500, $"An error occurred while fetching inventory assignments: {ex.Message}");
            }
        }

        [HttpGet("employee/{employeeId:int}")]
        public async Task<ActionResult<IEnumerable<EmployeeInventoryAssignmentsDto>>> GetAssignmentsByEmployeeId(int employeeId)
        {
            try
            {
                _logger.LogInformation("Retrieving inventory assignments for Employee ID {EmployeeID}.", employeeId);
                var assignments = await _inventoryService.GetAssignmentsByEmployeeIdAsync(employeeId);
                return Ok(assignments);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving inventory assignments for Employee ID {EmployeeID}.", employeeId);
                return StatusCode(500, $"An error occurred while fetching inventory assignments: {ex.Message}");
            }
        }

        [HttpGet("assignments/{id:int}")]
        public async Task<ActionResult<EmployeeInventoryAssignmentsDto>> GetAssignmentById(int id)
        {
            try
            {
                _logger.LogInformation("Retrieving inventory assignment ID {EmpInventoryID}.", id);
                var assignment = await _inventoryService.GetAssignmentByIdAsync(id);
                if (assignment == null) return NotFound($"Inventory assignment with ID {id} was not found.");

                return Ok(assignment);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving inventory assignment ID {EmpInventoryID}.", id);
                return StatusCode(500, $"An error occurred while fetching inventory assignment: {ex.Message}");
            }
        }

        [HttpPost("assignments")]
        [Authorize(Policy = "CanEditPersonnel")]
        public async Task<ActionResult<EmployeeInventoryAssignmentsDto>> CreateAssignment([FromBody] EmployeeInventoryAssignmentsDto dto)
        {
            if (dto == null) return BadRequest("Payload cannot be null.");

            try
            {
                _logger.LogInformation("Creating new inventory assignment for Employee ID {EmployeeID}.", dto.EmployeeID);
                var created = await _inventoryService.CreateAssignmentAsync(dto);
                return CreatedAtAction(nameof(GetAssignmentById), new { id = created.EmpInventoryID }, created);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating inventory assignment.");
                return StatusCode(500, $"An error occurred while creating inventory assignment: {ex.Message}");
            }
        }

        [HttpPost("assign-multiple")]
        [Authorize(Policy = "CanEditPersonnel")]
        public async Task<ActionResult<MultipleEquipmentAssignmentResultDto>> AssignMultipleEquipment([FromBody] MultipleEquipmentAssignmentRequestDto request)
        {
            if (request == null) return BadRequest("Payload cannot be null.");

            try
            {
                _logger.LogInformation("Assigning multiple equipment items to multiple employees.");
                var result = await _inventoryService.AssignMultipleEquipmentAsync(request);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error assigning multiple equipment items.");
                return StatusCode(500, $"An error occurred while assigning multiple equipment items: {ex.Message}");
            }
        }

        [HttpPut("assignments/{id:int}")]
        [Authorize(Policy = "CanEditPersonnel")]
        public async Task<ActionResult<EmployeeInventoryAssignmentsDto>> UpdateAssignment(int id, [FromBody] EmployeeInventoryAssignmentsDto dto)
        {
            if (dto == null) return BadRequest("Payload cannot be null.");

            try
            {
                _logger.LogInformation("Updating inventory assignment ID {EmpInventoryID}.", id);
                var updated = await _inventoryService.UpdateAssignmentAsync(id, dto);
                if (updated == null) return NotFound($"Inventory assignment with ID {id} was not found.");

                return Ok(updated);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating inventory assignment ID {EmpInventoryID}.", id);
                return StatusCode(500, $"An error occurred while updating inventory assignment: {ex.Message}");
            }
        }

        [HttpDelete("assignments/{id:int}")]
        [Authorize(Policy = "CanEditPersonnel")]
        public async Task<IActionResult> DeleteAssignment(int id)
        {
            try
            {
                _logger.LogInformation("Deleting inventory assignment ID {EmpInventoryID}.", id);
                var success = await _inventoryService.DeleteAssignmentAsync(id);
                if (!success) return NotFound($"Inventory assignment with ID {id} was not found.");

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting inventory assignment ID {EmpInventoryID}.", id);
                return StatusCode(500, $"An error occurred while deleting inventory assignment: {ex.Message}");
            }
        }

        [HttpGet("items")]
        public async Task<ActionResult<IEnumerable<InventoryItemsDto>>> GetInventoryItems([FromQuery] bool? onlyActive = null)
        {
            try
            {
                var items = await _inventoryService.GetInventoryItemsAsync(onlyActive);
                return Ok(items);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving inventory items.");
                return StatusCode(500, $"An error occurred while fetching inventory items: {ex.Message}");
            }
        }

        [HttpGet("items/{id:int}")]
        public async Task<ActionResult<InventoryItemsDto>> GetInventoryItemById(int id)
        {
            try
            {
                var item = await _inventoryService.GetInventoryItemByIdAsync(id);
                if (item == null) return NotFound($"Inventory item with ID {id} was not found.");

                return Ok(item);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving inventory item ID {InventoryID}.", id);
                return StatusCode(500, $"An error occurred while fetching inventory item: {ex.Message}");
            }
        }

        [HttpPost("items")]
        public async Task<ActionResult<InventoryItemsDto>> CreateInventoryItem([FromBody] InventoryItemsDto dto)
        {
            if (dto == null) return BadRequest("Payload cannot be null.");
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                _logger.LogInformation("Creating new inventory item: {ProductName}.", dto.ProductName);
                var created = await _inventoryService.CreateInventoryItemAsync(dto);
                return CreatedAtAction(nameof(GetInventoryItemById), new { id = created.InventoryID }, created);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating inventory item.");
                return StatusCode(500, $"An error occurred while creating inventory item: {ex.Message}");
            }
        }

        [HttpPut("items/{id:int}")]
        public async Task<ActionResult<InventoryItemsDto>> UpdateInventoryItem(int id, [FromBody] InventoryItemsDto dto)
        {
            if (dto == null) return BadRequest("Payload cannot be null.");
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                _logger.LogInformation("Updating inventory item ID {InventoryID}.", id);
                var updated = await _inventoryService.UpdateInventoryItemAsync(id, dto);
                if (updated == null) return NotFound($"Inventory item with ID {id} was not found.");

                return Ok(updated);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating inventory item ID {InventoryID}.", id);
                return StatusCode(500, $"An error occurred while updating inventory item: {ex.Message}");
            }
        }

        [HttpDelete("items/{id:int}")]
        public async Task<IActionResult> DeleteInventoryItem(int id)
        {
            try
            {
                _logger.LogInformation("Deleting inventory item ID {InventoryID}.", id);
                var success = await _inventoryService.DeleteInventoryItemAsync(id);
                if (!success) return NotFound($"Inventory item with ID {id} was not found.");

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting inventory item ID {InventoryID}.", id);
                return StatusCode(500, $"An error occurred while deleting inventory item: {ex.Message}");
            }
        }
    }
}

