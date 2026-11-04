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
    [Route("api/admin")]
    [Authorize(Roles = "Administrator")]
    public class AdminController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ILogger<AdminController> _logger;

        public AdminController(
            IUserService userService,
            ILogger<AdminController> logger)
        {
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpGet("users")]
        public async Task<ActionResult<IEnumerable<UserDTO>>> GetUsers()
        {
            try
            {
                _logger.LogInformation("Retrieving all users for admin management.");
                var users = await _userService.GetAllUsersAsync();
                return Ok(users);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving users.");
                return StatusCode(500, $"An error occurred while fetching users: {ex.Message}");
            }
        }

        [HttpGet("users/{id:int}")]
        public async Task<ActionResult<UserDTO>> GetUser(int id)
        {
            try
            {
                _logger.LogInformation("Retrieving user detail for UserID {UserID}.", id);
                var user = await _userService.GetUserByIdAsync(id);

                if (user == null)
                {
                    _logger.LogWarning("User with ID {UserID} was not found.", id);
                    return NotFound($"User with ID {id} was not found.");
                }

                return Ok(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving user ID {UserID}.", id);
                return StatusCode(500, $"An error occurred while fetching user: {ex.Message}");
            }
        }

        [HttpPost("users")]
        public async Task<ActionResult<UserDTO>> CreateUser([FromBody] UserCreateUpdateDto dto)
        {
            if (dto == null)
            {
                return BadRequest("User payload cannot be null.");
            }

            try
            {
                _logger.LogInformation("Creating new user {DisplayName} ({UPN}).", dto.DisplayName, dto.UserPrincipalName);
                var assignedBy = await GetLoggedInUserIdAsync();
                var created = await _userService.CreateUserAsync(dto, assignedBy);
                return CreatedAtAction(nameof(GetUser), new { id = created.UserID }, created);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating user.");
                return StatusCode(500, $"An error occurred while creating user: {ex.Message}");
            }
        }

        [HttpPut("users/{id:int}")]
        public async Task<ActionResult<UserDTO>> UpdateUser(int id, [FromBody] UserCreateUpdateDto dto)
        {
            if (dto == null)
            {
                return BadRequest("User payload cannot be null.");
            }

            try
            {
                _logger.LogInformation("Updating user ID {UserID}.", id);
                var assignedBy = await GetLoggedInUserIdAsync();
                var updated = await _userService.UpdateUserAsync(id, dto, assignedBy);

                if (updated == null)
                {
                    return NotFound($"User with ID {id} was not found.");
                }

                return Ok(updated);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating user ID {UserID}.", id);
                return StatusCode(500, $"An error occurred while updating user: {ex.Message}");
            }
        }

        private async Task<string> GetLoggedInUserIdAsync()
        {
            var userIdClaim = User.FindFirst("UserID")?.Value ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!string.IsNullOrEmpty(userIdClaim) && int.TryParse(userIdClaim, out _))
            {
                return userIdClaim;
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
                    return user.UserID.ToString();
                }
            }

            return "System Administrator";
        }

        [HttpDelete("users/{id:int}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            try
            {
                _logger.LogInformation("Deleting user ID {UserID}.", id);
                var success = await _userService.DeleteUserAsync(id);

                if (!success)
                {
                    return NotFound($"User with ID {id} was not found.");
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting user ID {UserID}.", id);
                return StatusCode(500, $"An error occurred while deleting user: {ex.Message}");
            }
        }

        [HttpGet("roles")]
        public async Task<ActionResult<IEnumerable<RoleDTO>>> GetRoles()
        {
            try
            {
                _logger.LogInformation("Retrieving system roles.");
                var roles = await _userService.GetRolesAsync();
                return Ok(roles);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving roles.");
                return StatusCode(500, $"An error occurred while fetching roles: {ex.Message}");
            }
        }
    }
}

