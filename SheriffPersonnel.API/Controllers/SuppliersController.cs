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
    public class SuppliersController : ControllerBase
    {
        private readonly ICodeTableService _codeTableService;
        private readonly ILogger<SuppliersController> _logger;

        public SuppliersController(
            ICodeTableService codeTableService,
            ILogger<SuppliersController> logger)
        {
            _codeTableService = codeTableService ?? throw new ArgumentNullException(nameof(codeTableService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<SuppliersDto>>> GetSuppliers()
        {
            try
            {
                var list = await _codeTableService.GetSuppliersAsync();
                return Ok(list);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching suppliers.");
                return StatusCode(500, $"Error fetching suppliers: {ex.Message}");
            }
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<SuppliersDto>> GetSupplier(int id)
        {
            try
            {
                var item = await _codeTableService.GetSupplierByIdAsync(id);
                if (item == null) return NotFound($"Supplier with ID {id} not found.");
                return Ok(item);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching supplier {ID}.", id);
                return StatusCode(500, $"Error fetching supplier: {ex.Message}");
            }
        }

        [HttpPost]
        public async Task<ActionResult<SuppliersDto>> CreateSupplier([FromBody] SuppliersDto dto)
        {
            if (dto == null) return BadRequest("Payload cannot be null.");
            try
            {
                var created = await _codeTableService.CreateSupplierAsync(dto);
                return CreatedAtAction(nameof(GetSupplier), new { id = created.SupplierID }, created);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating supplier.");
                return StatusCode(500, $"Error creating supplier: {ex.Message}");
            }
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<SuppliersDto>> UpdateSupplier(int id, [FromBody] SuppliersDto dto)
        {
            if (dto == null) return BadRequest("Payload cannot be null.");
            try
            {
                var updated = await _codeTableService.UpdateSupplierAsync(id, dto);
                if (updated == null) return NotFound($"Supplier with ID {id} not found.");
                return Ok(updated);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating supplier {ID}.", id);
                return StatusCode(500, $"Error updating supplier: {ex.Message}");
            }
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteSupplier(int id)
        {
            try
            {
                var success = await _codeTableService.DeleteSupplierAsync(id);
                if (!success) return NotFound($"Supplier with ID {id} not found.");
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting supplier {ID}.", id);
                return StatusCode(500, $"Error deleting supplier: {ex.Message}");
            }
        }
    }
}

