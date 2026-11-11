using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SheriffPersonnel.API.Services;
using SheriffPersonnel.Shared.DTOs;

namespace SheriffPersonnel.API.Controllers
{
    [ApiController]
    [Route("api/documents")]
    [Authorize(Roles = "Administrator, Personnel View, Personnel Edit, Inventory, Range Statistics, Range Scores, Training, Equipment, SSN")]
    public class DocumentsController : ControllerBase
    {
        private readonly IDocumentService _documentService;
        private readonly IUserService _userService;
        private readonly ILogger<DocumentsController> _logger;

        public DocumentsController(
            IDocumentService documentService,
            IUserService userService,
            ILogger<DocumentsController> logger)
        {
            _documentService = documentService ?? throw new ArgumentNullException(nameof(documentService));
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        private async Task<int> GetLoggedInUserIdAsync()
        {
            var userIdClaim = User.FindFirst("UserID")?.Value ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!string.IsNullOrEmpty(userIdClaim) && int.TryParse(userIdClaim, out int uid))
            {
                return uid;
            }

            var oid = User.FindFirst("oid")?.Value
                   ?? User.FindFirst("http://schemas.microsoft.com/identity/claims/objectidentifier")?.Value;

            var upn = User.FindFirst("preferred_username")?.Value
                   ?? User.FindFirst(ClaimTypes.Upn)?.Value
                   ?? User.FindFirst(ClaimTypes.Email)?.Value
                   ?? User.Identity?.Name;

            if (!string.IsNullOrEmpty(oid) || !string.IsNullOrEmpty(upn))
            {
                var user = await _userService.GetUserByEntraOrUpnAsync(oid, upn);
                if (user != null)
                {
                    return user.UserID;
                }
            }

            return 0;
        }

        [HttpGet("employee/{employeeId:int}")]
        public async Task<ActionResult<IEnumerable<EmployeeDocumentDto>>> GetDocumentsByEmployeeId(int employeeId)
        {
            try
            {
                _logger.LogInformation("Retrieving document records for Employee ID {EmployeeID}.", employeeId);
                var documents = await _documentService.GetDocumentsByEmployeeIdAsync(employeeId);
                return Ok(documents);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving document records for Employee ID {EmployeeID}.", employeeId);
                return StatusCode(500, $"An error occurred while fetching document records: {ex.Message}");
            }
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<EmployeeDocumentDto>> GetDocumentById(int id)
        {
            try
            {
                _logger.LogInformation("Retrieving document ID {DocumentID}.", id);
                var doc = await _documentService.GetDocumentByIdAsync(id);
                if (doc == null) return NotFound($"Document record with ID {id} was not found.");

                return Ok(doc);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving document ID {DocumentID}.", id);
                return StatusCode(500, $"An error occurred while fetching document record: {ex.Message}");
            }
        }

        [HttpGet("{id:int}/download")]
        public async Task<IActionResult> DownloadDocument(int id)
        {
            try
            {
                _logger.LogInformation("Downloading document content for ID {DocumentID}.", id);
                var doc = await _documentService.GetDocumentEntityByIdAsync(id);
                if (doc == null) return NotFound($"Document record with ID {id} was not found.");

                var mime = string.IsNullOrWhiteSpace(doc.MimeType) ? "application/octet-stream" : doc.MimeType;
                var fileName = string.IsNullOrWhiteSpace(doc.FileName) ? $"document_{id}" : doc.FileName;

                return File(doc.FileData, mime, fileName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error downloading document file ID {DocumentID}.", id);
                return StatusCode(500, $"An error occurred while downloading document file: {ex.Message}");
            }
        }

        [HttpPost]
        [Authorize(Policy = "CanEditPersonnel")]
        public async Task<ActionResult<EmployeeDocumentDto>> CreateDocument([FromBody] EmployeeDocumentDto dto)
        {
            if (dto == null) return BadRequest("Payload cannot be null.");

            try
            {
                if (dto.UploadedBy <= 0)
                {
                    dto.UploadedBy = await GetLoggedInUserIdAsync();
                }

                _logger.LogInformation("Creating new document record for Employee ID {EmployeeID} by User ID {UploadedBy}.", dto.EmployeeID, dto.UploadedBy);
                var created = await _documentService.CreateDocumentAsync(dto);
                return CreatedAtAction(nameof(GetDocumentById), new { id = created.DocumentID }, created);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating document record.");
                return StatusCode(500, $"An error occurred while creating document record: {ex.Message}");
            }
        }

        [HttpPut("{id:int}")]
        [Authorize(Policy = "CanEditPersonnel")]
        public async Task<ActionResult<EmployeeDocumentDto>> UpdateDocument(int id, [FromBody] EmployeeDocumentDto dto)
        {
            if (dto == null) return BadRequest("Payload cannot be null.");

            try
            {
                _logger.LogInformation("Updating document record ID {DocumentID}.", id);
                var updated = await _documentService.UpdateDocumentAsync(id, dto);
                if (updated == null) return NotFound($"Document record with ID {id} was not found.");

                return Ok(updated);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating document record ID {DocumentID}.", id);
                return StatusCode(500, $"An error occurred while updating document record: {ex.Message}");
            }
        }

        [HttpDelete("{id:int}")]
        [Authorize(Policy = "CanEditPersonnel")]
        public async Task<IActionResult> DeleteDocument(int id)
        {
            try
            {
                _logger.LogInformation("Deleting document record ID {DocumentID}.", id);
                var success = await _documentService.DeleteDocumentAsync(id);
                if (!success) return NotFound($"Document record with ID {id} was not found.");

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting document record ID {DocumentID}.", id);
                return StatusCode(500, $"An error occurred while deleting document record: {ex.Message}");
            }
        }

        [HttpGet("types")]
        public async Task<ActionResult<IEnumerable<DocumentTypeDto>>> GetDocumentTypes()
        {
            try
            {
                var types = await _documentService.GetDocumentTypesAsync();
                return Ok(types);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving document types.");
                return StatusCode(500, $"An error occurred while fetching document types: {ex.Message}");
            }
        }

        [HttpPost("types")]
        public async Task<ActionResult<DocumentTypeDto>> CreateDocumentType([FromBody] DocumentTypeDto dto)
        {
            if (dto == null) return BadRequest("Payload cannot be null.");

            try
            {
                var created = await _documentService.CreateDocumentTypeAsync(dto);
                return Ok(created);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating document type.");
                return StatusCode(500, $"An error occurred while creating document type: {ex.Message}");
            }
        }

        [HttpPut("types/{id:int}")]
        public async Task<ActionResult<DocumentTypeDto>> UpdateDocumentType(int id, [FromBody] DocumentTypeDto dto)
        {
            if (dto == null) return BadRequest("Payload cannot be null.");

            try
            {
                var updated = await _documentService.UpdateDocumentTypeAsync(id, dto);
                if (updated == null) return NotFound($"Document type with ID {id} was not found.");

                return Ok(updated);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating document type.");
                return StatusCode(500, $"An error occurred while updating document type: {ex.Message}");
            }
        }

        [HttpDelete("types/{id:int}")]
        public async Task<IActionResult> DeleteDocumentType(int id)
        {
            try
            {
                var success = await _documentService.DeleteDocumentTypeAsync(id);
                if (!success) return NotFound($"Document type with ID {id} was not found.");

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting document type.");
                return StatusCode(500, $"An error occurred while deleting document type: {ex.Message}");
            }
        }
    }
}

