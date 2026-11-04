using Microsoft.Identity.Abstractions;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Identity.Web;
using SheriffPersonnel.Shared.DTOs;

namespace SheriffPersonnel.Web.Services
{
    public class UserAdminService
    {
        private readonly IDownstreamApi _downstreamApi;
        private readonly MicrosoftIdentityConsentAndConditionalAccessHandler _consentHandler;
        private readonly ILogger<UserAdminService> _logger;

        public UserAdminService(
            IDownstreamApi downstreamApi,
            MicrosoftIdentityConsentAndConditionalAccessHandler consentHandler,
            ILogger<UserAdminService> logger)
        {
            _downstreamApi = downstreamApi ?? throw new ArgumentNullException(nameof(downstreamApi));
            _consentHandler = consentHandler ?? throw new ArgumentNullException(nameof(consentHandler));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<IEnumerable<UserDTO>> GetUsersAsync()
        {
            try
            {
                _logger.LogInformation("Invoking downstream API to retrieve users.");
                var response = await _downstreamApi.CallApiForUserAsync<IEnumerable<UserDTO>>(
                    "MyCustomApi",
                    options =>
                    {
                        options.HttpMethod = HttpMethod.Get.Method;
                        options.RelativePath = "api/admin/users";
                    });

                return response ?? Array.Empty<UserDTO>();
            }
            catch (MicrosoftIdentityWebChallengeUserException ex)
            {
                _logger.LogWarning(ex, "Entra ID user challenge/consent required during GetUsersAsync.");
                _consentHandler.HandleException(ex);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving users via downstream API.");
                throw;
            }
        }

        public async Task<UserDTO?> GetUserByIdAsync(int id)
        {
            try
            {
                _logger.LogInformation("Invoking downstream API to retrieve user ID {UserID}.", id);
                return await _downstreamApi.CallApiForUserAsync<UserDTO>(
                    "MyCustomApi",
                    options =>
                    {
                        options.HttpMethod = HttpMethod.Get.Method;
                        options.RelativePath = $"api/admin/users/{id}";
                    });
            }
            catch (MicrosoftIdentityWebChallengeUserException ex)
            {
                _consentHandler.HandleException(ex);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving user ID {UserID} via downstream API.", id);
                throw;
            }
        }

        public async Task<UserDTO?> CreateUserAsync(UserCreateUpdateDto dto)
        {
            try
            {
                _logger.LogInformation("Invoking downstream API to create user {DisplayName}.", dto.DisplayName);
                return await _downstreamApi.CallApiForUserAsync<UserCreateUpdateDto, UserDTO>(
                    "MyCustomApi",
                    dto,
                    options =>
                    {
                        options.HttpMethod = HttpMethod.Post.Method;
                        options.RelativePath = "api/admin/users";
                    });
            }
            catch (MicrosoftIdentityWebChallengeUserException ex)
            {
                _consentHandler.HandleException(ex);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating user via downstream API.");
                throw;
            }
        }

        public async Task<UserDTO?> UpdateUserAsync(int id, UserCreateUpdateDto dto)
        {
            try
            {
                _logger.LogInformation("Invoking downstream API to update user ID {UserID}.", id);
                return await _downstreamApi.CallApiForUserAsync<UserCreateUpdateDto, UserDTO>(
                    "MyCustomApi",
                    dto,
                    options =>
                    {
                        options.HttpMethod = HttpMethod.Put.Method;
                        options.RelativePath = $"api/admin/users/{id}";
                    });
            }
            catch (MicrosoftIdentityWebChallengeUserException ex)
            {
                _consentHandler.HandleException(ex);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user ID {UserID} via downstream API.", id);
                throw;
            }
        }

        public async Task<bool> DeleteUserAsync(int id)
        {
            try
            {
                _logger.LogInformation("Invoking downstream API to delete user ID {UserID}.", id);
                var response = await _downstreamApi.CallApiForUserAsync(
                    "MyCustomApi",
                    options =>
                    {
                        options.HttpMethod = HttpMethod.Delete.Method;
                        options.RelativePath = $"api/admin/users/{id}";
                    });
                return response?.IsSuccessStatusCode ?? false;
            }
            catch (MicrosoftIdentityWebChallengeUserException ex)
            {
                _consentHandler.HandleException(ex);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting user ID {UserID} via downstream API.", id);
                return false;
            }
        }

        public async Task<IEnumerable<RoleDTO>> GetRolesAsync()
        {
            try
            {
                _logger.LogInformation("Invoking downstream API to retrieve roles.");
                var response = await _downstreamApi.CallApiForUserAsync<IEnumerable<RoleDTO>>(
                    "MyCustomApi",
                    options =>
                    {
                        options.HttpMethod = HttpMethod.Get.Method;
                        options.RelativePath = "api/admin/roles";
                    });

                return response ?? Array.Empty<RoleDTO>();
            }
            catch (MicrosoftIdentityWebChallengeUserException ex)
            {
                _consentHandler.HandleException(ex);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving roles via downstream API.");
                return Array.Empty<RoleDTO>();
            }
        }
    }
}

