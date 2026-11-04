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
    public class CategoriesController : ControllerBase
    {
        private readonly ICodeTableService _codeTableService;
        private readonly ILogger<CategoriesController> _logger;

        public CategoriesController(
            ICodeTableService codeTableService,
            ILogger<CategoriesController> logger)
        {
            _codeTableService = codeTableService ?? throw new ArgumentNullException(nameof(codeTableService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CategoriesDto>>> GetCategories()
        {
            try
            {
                var list = await _codeTableService.GetCategoriesAsync();
                return Ok(list);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching categories.");
                return StatusCode(500, $"Error fetching categories: {ex.Message}");
            }
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<CategoriesDto>> GetCategory(int id)
        {
            try
            {
                var item = await _codeTableService.GetCategoryByIdAsync(id);
                if (item == null) return NotFound($"Category with ID {id} not found.");
                return Ok(item);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching category {ID}.", id);
                return StatusCode(500, $"Error fetching category: {ex.Message}");
            }
        }

        [HttpPost]
        public async Task<ActionResult<CategoriesDto>> CreateCategory([FromBody] CategoriesDto dto)
        {
            if (dto == null) return BadRequest("Payload cannot be null.");
            try
            {
                var created = await _codeTableService.CreateCategoryAsync(dto);
                return CreatedAtAction(nameof(GetCategory), new { id = created.CategoryID }, created);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating category.");
                return StatusCode(500, $"Error creating category: {ex.Message}");
            }
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<CategoriesDto>> UpdateCategory(int id, [FromBody] CategoriesDto dto)
        {
            if (dto == null) return BadRequest("Payload cannot be null.");
            try
            {
                var updated = await _codeTableService.UpdateCategoryAsync(id, dto);
                if (updated == null) return NotFound($"Category with ID {id} not found.");
                return Ok(updated);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating category {ID}.", id);
                return StatusCode(500, $"Error updating category: {ex.Message}");
            }
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            try
            {
                var success = await _codeTableService.DeleteCategoryAsync(id);
                if (!success) return NotFound($"Category with ID {id} not found.");
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting category {ID}.", id);
                return StatusCode(500, $"Error deleting category: {ex.Message}");
            }
        }
    }
}

