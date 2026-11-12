using Microsoft.Extensions.Caching.Memory;
using Microsoft.Identity.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Identity.Web;
using SheriffPersonnel.Shared.DTOs;

namespace SheriffPersonnel.Web.Services
{
    public class SearchService
    {
        private readonly IDownstreamApi _downstreamApi;
        private readonly MicrosoftIdentityConsentAndConditionalAccessHandler _consentHandler;
        private readonly Microsoft.Extensions.Logging.ILogger<SearchService> _logger;
        private readonly IMemoryCache? _cache;

        public SearchService(
            IDownstreamApi downstreamApi,
            MicrosoftIdentityConsentAndConditionalAccessHandler consentHandler,
            Microsoft.Extensions.Logging.ILogger<SearchService> logger,
            IMemoryCache? cache = null)
        {
            _downstreamApi = downstreamApi ?? throw new ArgumentNullException(nameof(downstreamApi));
            _consentHandler = consentHandler ?? throw new ArgumentNullException(nameof(consentHandler));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _cache = cache;
        }

        private async Task<IEnumerable<T>> GetCachedOrFetchAsync<T>(string cacheKey, Func<Task<IEnumerable<T>>> fetchFunc, TimeSpan? duration = null)
        {
            if (_cache != null && _cache.TryGetValue(cacheKey, out IEnumerable<T>? cached) && cached != null)
            {
                return cached;
            }

            var result = await fetchFunc();
            var list = result as List<T> ?? result.ToList();
            if (_cache != null && list.Count > 0)
            {
                _cache.Set(cacheKey, (IEnumerable<T>)list, duration ?? TimeSpan.FromMinutes(30));
            }
            return list;
        }

        public async Task<IEnumerable<EmployeeSearchResultDto>> SearchEmployeesAsync(EmployeeSearchRequestDto request)
        {
            try
            {
                _logger.LogInformation("Invoking downstream API for employee search.");

                var response = await _downstreamApi.CallApiForUserAsync<EmployeeSearchRequestDto, IEnumerable<EmployeeSearchResultDto>>(
                    "MyCustomApi",
                    request,
                    options =>
                    {
                        options.HttpMethod = HttpMethod.Post.Method;
                        options.RelativePath = "api/employee/search";
                    });

                return response ?? Array.Empty<EmployeeSearchResultDto>();
            }
            catch (MicrosoftIdentityWebChallengeUserException ex)
            {
                _logger.LogWarning(ex, "Entra ID user challenge/consent required during SearchEmployeesAsync.");
                _consentHandler.HandleException(ex);
                throw;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "HTTP request failure during SearchEmployeesAsync downstream API call. StatusCode: {StatusCode}", ex.StatusCode);
                throw new Exception("Unable to reach employee search API service. Please verify network connectivity or try again.", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled error occurred in SearchEmployeesAsync.");
                throw;
            }
        }

        public async Task<IEnumerable<StatusesDto>> GetAvailableStatusesAsync()
        {
            return await GetCachedOrFetchAsync("SearchService_Statuses", async () =>
            {
                try
                {
                    _logger.LogInformation("Invoking downstream API for available employee statuses.");

                    var response = await _downstreamApi.CallApiForUserAsync<IEnumerable<StatusesDto>>(
                        "MyCustomApi",
                        options =>
                        {
                            options.HttpMethod = HttpMethod.Get.Method;
                            options.RelativePath = "api/employee/search/statuses";
                        });

                    return response ?? Array.Empty<StatusesDto>();
                }
                catch (MicrosoftIdentityWebChallengeUserException ex)
                {
                    _logger.LogWarning(ex, "Entra ID user challenge/consent required during GetAvailableStatusesAsync.");
                    _consentHandler.HandleException(ex);
                    throw;
                }
                catch (HttpRequestException ex)
                {
                    _logger.LogError(ex, "HTTP request failure during GetAvailableStatusesAsync downstream API call. StatusCode: {StatusCode}", ex.StatusCode);
                    return Array.Empty<StatusesDto>();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Unhandled error occurred in GetAvailableStatusesAsync.");
                    return Array.Empty<StatusesDto>();
                }
            });
        }

        public async Task<EmployeeDetailDto?> GetEmployeeByIdAsync(int id)
        {
            try
            {
                _logger.LogInformation("Invoking downstream API to get employee ID {EmployeeID}.", id);
                return await _downstreamApi.CallApiForUserAsync<EmployeeDetailDto>(
                    "MyCustomApi",
                    options =>
                    {
                        options.HttpMethod = HttpMethod.Get.Method;
                        options.RelativePath = $"api/employee/{id}";
                    });
            }
            catch (MicrosoftIdentityWebChallengeUserException ex)
            {
                _consentHandler.HandleException(ex);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get employee ID {EmployeeID} from downstream API.", id);
                throw;
            }
        }

        public async Task<EmployeeDetailDto?> CreateEmployeeAsync(EmployeeDetailDto dto)
        {
            try
            {
                _logger.LogInformation("Invoking downstream API to create employee.");
                return await _downstreamApi.CallApiForUserAsync<EmployeeDetailDto, EmployeeDetailDto>(
                    "MyCustomApi",
                    dto,
                    options =>
                    {
                        options.HttpMethod = HttpMethod.Post.Method;
                        options.RelativePath = "api/employee";
                    });
            }
            catch (MicrosoftIdentityWebChallengeUserException ex)
            {
                _consentHandler.HandleException(ex);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create employee via downstream API.");
                throw;
            }
        }

        public async Task<EmployeeDetailDto?> UpdateEmployeeAsync(int id, EmployeeDetailDto dto)
        {
            try
            {
                _logger.LogInformation("Invoking downstream API to update employee ID {EmployeeID}.", id);
                return await _downstreamApi.CallApiForUserAsync<EmployeeDetailDto, EmployeeDetailDto>(
                    "MyCustomApi",
                    dto,
                    options =>
                    {
                        options.HttpMethod = HttpMethod.Put.Method;
                        options.RelativePath = $"api/employee/{id}";
                    });
            }
            catch (MicrosoftIdentityWebChallengeUserException ex)
            {
                _consentHandler.HandleException(ex);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update employee ID {EmployeeID} via downstream API.", id);
                throw;
            }
        }

        public async Task<bool> DeleteEmployeeAsync(int id)
        {
            try
            {
                _logger.LogInformation("Invoking downstream API to delete employee ID {EmployeeID}.", id);
                var response = await _downstreamApi.CallApiForUserAsync(
                    "MyCustomApi",
                    options =>
                    {
                        options.HttpMethod = HttpMethod.Delete.Method;
                        options.RelativePath = $"api/employee/{id}";
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
                _logger.LogError(ex, "Failed to delete employee ID {EmployeeID} via downstream API.", id);
                return false;
            }
        }

        public async Task<IEnumerable<EmployeeChangesDto>> GetEmployeeChangesByEmployeeIdAsync(int employeeId)
        {
            try
            {
                _logger.LogInformation("Invoking downstream API for employee changes for Employee ID {EmployeeID}.", employeeId);

                var response = await _downstreamApi.CallApiForUserAsync<IEnumerable<EmployeeChangesDto>>(
                    "MyCustomApi",
                    options =>
                    {
                        options.HttpMethod = HttpMethod.Get.Method;
                        options.RelativePath = $"api/employeechanges/employee/{employeeId}";
                    });

                return response ?? Array.Empty<EmployeeChangesDto>();
            }
            catch (MicrosoftIdentityWebChallengeUserException ex)
            {
                _consentHandler.HandleException(ex);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calling downstream API for employee changes.");
                return Array.Empty<EmployeeChangesDto>();
            }
        }

        public async Task<EmployeeChangesDto?> CreateEmployeeChangeAsync(EmployeeChangesDto dto)
        {
            try
            {
                _logger.LogInformation("Invoking downstream API to create employee change for Employee ID {EmployeeID}.", dto.EmployeeID);
                return await _downstreamApi.CallApiForUserAsync<EmployeeChangesDto, EmployeeChangesDto>(
                    "MyCustomApi",
                    dto,
                    options =>
                    {
                        options.HttpMethod = HttpMethod.Post.Method;
                        options.RelativePath = "api/employeechanges";
                    });
            }
            catch (MicrosoftIdentityWebChallengeUserException ex)
            {
                _consentHandler.HandleException(ex);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating employee change via downstream API.");
                throw;
            }
        }

        public async Task<EmployeeChangesDto?> UpdateEmployeeChangeAsync(int id, EmployeeChangesDto dto)
        {
            try
            {
                _logger.LogInformation("Invoking downstream API to update employee change ID {HistoryID}.", id);
                return await _downstreamApi.CallApiForUserAsync<EmployeeChangesDto, EmployeeChangesDto>(
                    "MyCustomApi",
                    dto,
                    options =>
                    {
                        options.HttpMethod = HttpMethod.Put.Method;
                        options.RelativePath = $"api/employeechanges/{id}";
                    });
            }
            catch (MicrosoftIdentityWebChallengeUserException ex)
            {
                _consentHandler.HandleException(ex);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating employee change via downstream API.");
                throw;
            }
        }

        public async Task<bool> DeleteEmployeeChangeAsync(int id)
        {
            try
            {
                _logger.LogInformation("Invoking downstream API to delete employee change ID {HistoryID}.", id);
                var response = await _downstreamApi.CallApiForUserAsync(
                    "MyCustomApi",
                    options =>
                    {
                        options.HttpMethod = HttpMethod.Delete.Method;
                        options.RelativePath = $"api/employeechanges/{id}";
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
                _logger.LogError(ex, "Error deleting employee change via downstream API.");
                return false;
            }
        }

        public async Task<IEnumerable<ProgramsDto>> GetProgramsAsync()
        {
            return await GetCachedOrFetchAsync("SearchService_Programs", async () =>
            {
                try
                {
                    var response = await _downstreamApi.CallApiForUserAsync<IEnumerable<ProgramsDto>>(
                        "MyCustomApi",
                        options =>
                        {
                            options.HttpMethod = HttpMethod.Get.Method;
                            options.RelativePath = "api/employeechanges/programs";
                        });
                    return response ?? Array.Empty<ProgramsDto>();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error fetching programs via downstream API.");
                    return Array.Empty<ProgramsDto>();
                }
            });
        }

        public async Task<IEnumerable<RanksDto>> GetRanksAsync()
        {
            return await GetCachedOrFetchAsync("SearchService_Ranks", async () =>
            {
                try
                {
                    var response = await _downstreamApi.CallApiForUserAsync<IEnumerable<RanksDto>>(
                        "MyCustomApi",
                        options =>
                        {
                            options.HttpMethod = HttpMethod.Get.Method;
                            options.RelativePath = "api/employeechanges/ranks";
                        });
                    return response ?? Array.Empty<RanksDto>();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error fetching ranks via downstream API.");
                    return Array.Empty<RanksDto>();
                }
            });
        }

        public async Task<IEnumerable<UnitsDto>> GetUnitsAsync()
        {
            return await GetCachedOrFetchAsync("SearchService_Units", async () =>
            {
                try
                {
                    var response = await _downstreamApi.CallApiForUserAsync<IEnumerable<UnitsDto>>(
                        "MyCustomApi",
                        options =>
                        {
                            options.HttpMethod = HttpMethod.Get.Method;
                            options.RelativePath = "api/employeechanges/units";
                        });
                    return response ?? Array.Empty<UnitsDto>();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error fetching units via downstream API.");
                    return Array.Empty<UnitsDto>();
                }
            });
        }

        public async Task<IEnumerable<EmployeeInventoryAssignmentsDto>> GetAllInventoryAssignmentsAsync(
            string? productName = null,
            int? programId = null,
            int? statusId = null,
            DateTime? startDate = null,
            DateTime? endDate = null,
            int? maxResults = null)
        {
            try
            {
                _logger.LogInformation("Invoking downstream API for inventory assignments.");

                var queryParams = new List<string>();
                if (!string.IsNullOrWhiteSpace(productName)) queryParams.Add($"productName={Uri.EscapeDataString(productName.Trim())}");
                if (programId.HasValue && programId.Value > 0) queryParams.Add($"programId={programId.Value}");
                if (statusId.HasValue && statusId.Value > 0) queryParams.Add($"statusId={statusId.Value}");
                if (startDate.HasValue) queryParams.Add($"startDate={startDate.Value:yyyy-MM-dd}");
                if (endDate.HasValue) queryParams.Add($"endDate={endDate.Value:yyyy-MM-dd}");
                if (maxResults.HasValue && maxResults.Value > 0) queryParams.Add($"maxResults={maxResults.Value}");

                var queryString = queryParams.Count > 0 ? "?" + string.Join("&", queryParams) : "";
                var relativePath = $"api/inventory/assignments{queryString}";

                var response = await _downstreamApi.CallApiForUserAsync<IEnumerable<EmployeeInventoryAssignmentsDto>>(
                    "MyCustomApi",
                    options =>
                    {
                        options.HttpMethod = HttpMethod.Get.Method;
                        options.RelativePath = relativePath;
                    });

                return response ?? Array.Empty<EmployeeInventoryAssignmentsDto>();
            }
            catch (MicrosoftIdentityWebChallengeUserException ex)
            {
                _consentHandler.HandleException(ex);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calling downstream API for inventory assignments.");
                return Array.Empty<EmployeeInventoryAssignmentsDto>();
            }
        }

        public async Task<IEnumerable<EmployeeInventoryAssignmentsDto>> GetEmployeeInventoryAssignmentsByEmployeeIdAsync(int employeeId)
        {
            try
            {
                _logger.LogInformation("Invoking downstream API for employee inventory assignments for Employee ID {EmployeeID}.", employeeId);

                var response = await _downstreamApi.CallApiForUserAsync<IEnumerable<EmployeeInventoryAssignmentsDto>>(
                    "MyCustomApi",
                    options =>
                    {
                        options.HttpMethod = HttpMethod.Get.Method;
                        options.RelativePath = $"api/inventory/employee/{employeeId}";
                    });

                return response ?? Array.Empty<EmployeeInventoryAssignmentsDto>();
            }
            catch (MicrosoftIdentityWebChallengeUserException ex)
            {
                _consentHandler.HandleException(ex);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calling downstream API for employee inventory assignments.");
                return Array.Empty<EmployeeInventoryAssignmentsDto>();
            }
        }

        public async Task<EmployeeInventoryAssignmentsDto?> CreateEmployeeInventoryAssignmentAsync(EmployeeInventoryAssignmentsDto dto)
        {
            try
            {
                _logger.LogInformation("Invoking downstream API to create inventory assignment for Employee ID {EmployeeID}.", dto.EmployeeID);
                return await _downstreamApi.CallApiForUserAsync<EmployeeInventoryAssignmentsDto, EmployeeInventoryAssignmentsDto>(
                    "MyCustomApi",
                    dto,
                    options =>
                    {
                        options.HttpMethod = HttpMethod.Post.Method;
                        options.RelativePath = "api/inventory/assignments";
                    });
            }
            catch (MicrosoftIdentityWebChallengeUserException ex)
            {
                _consentHandler.HandleException(ex);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating inventory assignment via downstream API.");
                throw;
            }
        }

        public async Task<EmployeeInventoryAssignmentsDto?> UpdateEmployeeInventoryAssignmentAsync(int id, EmployeeInventoryAssignmentsDto dto)
        {
            try
            {
                _logger.LogInformation("Invoking downstream API to update inventory assignment ID {EmpInventoryID}.", id);
                return await _downstreamApi.CallApiForUserAsync<EmployeeInventoryAssignmentsDto, EmployeeInventoryAssignmentsDto>(
                    "MyCustomApi",
                    dto,
                    options =>
                    {
                        options.HttpMethod = HttpMethod.Put.Method;
                        options.RelativePath = $"api/inventory/assignments/{id}";
                    });
            }
            catch (MicrosoftIdentityWebChallengeUserException ex)
            {
                _consentHandler.HandleException(ex);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating inventory assignment via downstream API.");
                throw;
            }
        }

        public async Task<bool> DeleteEmployeeInventoryAssignmentAsync(int id)
        {
            try
            {
                _logger.LogInformation("Invoking downstream API to delete inventory assignment ID {EmpInventoryID}.", id);
                var response = await _downstreamApi.CallApiForUserAsync(
                    "MyCustomApi",
                    options =>
                    {
                        options.HttpMethod = HttpMethod.Delete.Method;
                        options.RelativePath = $"api/inventory/assignments/{id}";
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
                _logger.LogError(ex, "Error deleting inventory assignment via downstream API.");
                return false;
            }
        }

        public async Task<IEnumerable<InventoryItemsDto>> GetInventoryItemsAsync(bool? onlyActive = null)
        {
            try
            {
                var relativePath = "api/inventory/items";
                if (onlyActive.HasValue)
                {
                    relativePath += $"?onlyActive={onlyActive.Value.ToString().ToLowerInvariant()}";
                }

                var response = await _downstreamApi.CallApiForUserAsync<IEnumerable<InventoryItemsDto>>(
                    "MyCustomApi",
                    options =>
                    {
                        options.HttpMethod = HttpMethod.Get.Method;
                        options.RelativePath = relativePath;
                    });
                return response ?? Array.Empty<InventoryItemsDto>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching inventory items via downstream API.");
                return Array.Empty<InventoryItemsDto>();
            }
        }

        public async Task<InventoryItemsDto?> GetInventoryItemByIdAsync(int id)
        {
            try
            {
                _logger.LogInformation("Invoking downstream API for inventory item ID {InventoryID}.", id);
                return await _downstreamApi.CallApiForUserAsync<InventoryItemsDto>(
                    "MyCustomApi",
                    options =>
                    {
                        options.HttpMethod = HttpMethod.Get.Method;
                        options.RelativePath = $"api/inventory/items/{id}";
                    });
            }
            catch (MicrosoftIdentityWebChallengeUserException ex)
            {
                _consentHandler.HandleException(ex);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching inventory item ID {InventoryID} via downstream API.", id);
                throw;
            }
        }

        public async Task<InventoryItemsDto?> CreateInventoryItemAsync(InventoryItemsDto dto)
        {
            try
            {
                _logger.LogInformation("Invoking downstream API to create inventory item {ProductName}.", dto.ProductName);
                return await _downstreamApi.CallApiForUserAsync<InventoryItemsDto, InventoryItemsDto>(
                    "MyCustomApi",
                    dto,
                    options =>
                    {
                        options.HttpMethod = HttpMethod.Post.Method;
                        options.RelativePath = "api/inventory/items";
                    });
            }
            catch (MicrosoftIdentityWebChallengeUserException ex)
            {
                _consentHandler.HandleException(ex);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating inventory item via downstream API.");
                throw;
            }
        }

        public async Task<InventoryItemsDto?> UpdateInventoryItemAsync(int id, InventoryItemsDto dto)
        {
            try
            {
                _logger.LogInformation("Invoking downstream API to update inventory item ID {InventoryID}.", id);
                return await _downstreamApi.CallApiForUserAsync<InventoryItemsDto, InventoryItemsDto>(
                    "MyCustomApi",
                    dto,
                    options =>
                    {
                        options.HttpMethod = HttpMethod.Put.Method;
                        options.RelativePath = $"api/inventory/items/{id}";
                    });
            }
            catch (MicrosoftIdentityWebChallengeUserException ex)
            {
                _consentHandler.HandleException(ex);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating inventory item via downstream API.");
                throw;
            }
        }

        public async Task<bool> DeleteInventoryItemAsync(int id)
        {
            try
            {
                _logger.LogInformation("Invoking downstream API to delete inventory item ID {InventoryID}.", id);
                var response = await _downstreamApi.CallApiForUserAsync(
                    "MyCustomApi",
                    options =>
                    {
                        options.HttpMethod = HttpMethod.Delete.Method;
                        options.RelativePath = $"api/inventory/items/{id}";
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
                _logger.LogError(ex, "Error deleting inventory item via downstream API.");
                return false;
            }
        }

        public async Task<IEnumerable<FirearmsDto>> GetAllFirearmsAsync()
        {
            try
            {
                _logger.LogInformation("Invoking downstream API for all firearms.");

                var response = await _downstreamApi.CallApiForUserAsync<IEnumerable<FirearmsDto>>(
                    "MyCustomApi",
                    options =>
                    {
                        options.HttpMethod = HttpMethod.Get.Method;
                        options.RelativePath = "api/firearms";
                    });

                return response ?? Array.Empty<FirearmsDto>();
            }
            catch (MicrosoftIdentityWebChallengeUserException ex)
            {
                _consentHandler.HandleException(ex);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calling downstream API for all firearms.");
                return Array.Empty<FirearmsDto>();
            }
        }

        public async Task<IEnumerable<FirearmsDto>> GetFirearmsByEmployeeIdAsync(int employeeId)
        {
            try
            {
                _logger.LogInformation("Invoking downstream API for firearms for Employee ID {EmployeeID}.", employeeId);

                var response = await _downstreamApi.CallApiForUserAsync<IEnumerable<FirearmsDto>>(
                    "MyCustomApi",
                    options =>
                    {
                        options.HttpMethod = HttpMethod.Get.Method;
                        options.RelativePath = $"api/firearms/employee/{employeeId}";
                    });

                return response ?? Array.Empty<FirearmsDto>();
            }
            catch (MicrosoftIdentityWebChallengeUserException ex)
            {
                _consentHandler.HandleException(ex);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calling downstream API for employee firearms.");
                return Array.Empty<FirearmsDto>();
            }
        }

        public async Task<FirearmsDto?> CreateFirearmAsync(FirearmsDto dto)
        {
            try
            {
                _logger.LogInformation("Invoking downstream API to create firearm for Employee ID {EmployeeID}.", dto.EmployeeID);
                return await _downstreamApi.CallApiForUserAsync<FirearmsDto, FirearmsDto>(
                    "MyCustomApi",
                    dto,
                    options =>
                    {
                        options.HttpMethod = HttpMethod.Post.Method;
                        options.RelativePath = "api/firearms";
                    });
            }
            catch (MicrosoftIdentityWebChallengeUserException ex)
            {
                _consentHandler.HandleException(ex);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating firearm via downstream API.");
                throw;
            }
        }

        public async Task<FirearmsDto?> UpdateFirearmAsync(int id, FirearmsDto dto)
        {
            try
            {
                _logger.LogInformation("Invoking downstream API to update firearm ID {FirearmID}.", id);
                return await _downstreamApi.CallApiForUserAsync<FirearmsDto, FirearmsDto>(
                    "MyCustomApi",
                    dto,
                    options =>
                    {
                        options.HttpMethod = HttpMethod.Put.Method;
                        options.RelativePath = $"api/firearms/{id}";
                    });
            }
            catch (MicrosoftIdentityWebChallengeUserException ex)
            {
                _consentHandler.HandleException(ex);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating firearm via downstream API.");
                throw;
            }
        }

        public async Task<bool> DeleteFirearmAsync(int id)
        {
            try
            {
                _logger.LogInformation("Invoking downstream API to delete firearm ID {FirearmID}.", id);
                var response = await _downstreamApi.CallApiForUserAsync(
                    "MyCustomApi",
                    options =>
                    {
                        options.HttpMethod = HttpMethod.Delete.Method;
                        options.RelativePath = $"api/firearms/{id}";
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
                _logger.LogError(ex, "Error deleting firearm via downstream API.");
                return false;
            }
        }

        public async Task<IEnumerable<MakesDto>> GetMakesAsync()
        {
            return await GetCachedOrFetchAsync("SearchService_Makes", async () =>
            {
                try
                {
                    var response = await _downstreamApi.CallApiForUserAsync<IEnumerable<MakesDto>>(
                        "MyCustomApi",
                        options =>
                        {
                            options.HttpMethod = HttpMethod.Get.Method;
                            options.RelativePath = "api/firearms/makes";
                        });
                    return response ?? Array.Empty<MakesDto>();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error fetching firearm makes via downstream API.");
                    return Array.Empty<MakesDto>();
                }
            });
        }

        public async Task<IEnumerable<RangeStatisticDTO>> GetRangeStatisticsAsync(
            int? courseId = null,
            int? employeeStatusId = null,
            int? programId = null,
            IEnumerable<int>? unitIds = null,
            DateOnly? startDate = null,
            DateOnly? endDate = null,
            string? passFail = null)
        {
            try
            {
                var queryParams = new List<string>();
                if (courseId.HasValue && courseId.Value > 0) queryParams.Add($"courseId={courseId.Value}");
                if (employeeStatusId.HasValue && employeeStatusId.Value > 0) queryParams.Add($"employeeStatusId={employeeStatusId.Value}");
                if (programId.HasValue && programId.Value > 0) queryParams.Add($"programId={programId.Value}");
                if (unitIds != null)
                {
                    foreach (var uid in unitIds.Where(u => u > 0))
                    {
                        queryParams.Add($"unitIds={uid}");
                    }
                }
                if (startDate.HasValue) queryParams.Add($"startDate={startDate.Value:yyyy-MM-dd}");
                if (endDate.HasValue) queryParams.Add($"endDate={endDate.Value:yyyy-MM-dd}");
                if (!string.IsNullOrWhiteSpace(passFail)) queryParams.Add($"passFail={Uri.EscapeDataString(passFail.Trim())}");

                var queryString = queryParams.Count > 0 ? "?" + string.Join("&", queryParams) : "";
                var relativePath = $"api/rangestatistics{queryString}";

                _logger.LogInformation("Invoking downstream API for range statistics with path: {RelativePath}", relativePath);
                var response = await _downstreamApi.CallApiForUserAsync<IEnumerable<RangeStatisticDTO>>(
                    "MyCustomApi",
                    options =>
                    {
                        options.HttpMethod = HttpMethod.Get.Method;
                        options.RelativePath = relativePath;
                    });

                return response ?? Array.Empty<RangeStatisticDTO>();
            }
            catch (MicrosoftIdentityWebChallengeUserException ex)
            {
                _consentHandler.HandleException(ex);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching range statistics via downstream API.");
                return Array.Empty<RangeStatisticDTO>();
            }
        }

        public async Task<IEnumerable<RangeStatisticDTO>> GetAllRangeStatisticsAsync()
        {
            return await GetRangeStatisticsAsync();
        }

        public async Task<IEnumerable<RangeStatisticDTO>> GetRangeStatisticsByEmployeeIdAsync(int employeeId)
        {
            try
            {
                _logger.LogInformation("Invoking downstream API for range statistics for Employee ID {EmployeeID}.", employeeId);
                var response = await _downstreamApi.CallApiForUserAsync<IEnumerable<RangeStatisticDTO>>(
                    "MyCustomApi",
                    options =>
                    {
                        options.HttpMethod = HttpMethod.Get.Method;
                        options.RelativePath = $"api/rangestatistics/employee/{employeeId}";
                    });

                return response ?? Array.Empty<RangeStatisticDTO>();
            }
            catch (MicrosoftIdentityWebChallengeUserException ex)
            {
                _consentHandler.HandleException(ex);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching employee range statistics via downstream API.");
                return Array.Empty<RangeStatisticDTO>();
            }
        }

        public async Task<RangeStatisticDTO?> GetRangeStatisticByIdAsync(int id)
        {
            try
            {
                _logger.LogInformation("Invoking downstream API for range statistic ID {RangeID}.", id);
                return await _downstreamApi.CallApiForUserAsync<RangeStatisticDTO>(
                    "MyCustomApi",
                    options =>
                    {
                        options.HttpMethod = HttpMethod.Get.Method;
                        options.RelativePath = $"api/rangestatistics/{id}";
                    });
            }
            catch (MicrosoftIdentityWebChallengeUserException ex)
            {
                _consentHandler.HandleException(ex);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching range statistic ID {RangeID} via downstream API.", id);
                throw;
            }
        }

        public async Task<RangeStatisticDTO?> CreateRangeStatisticAsync(RangeStatisticDTO dto)
        {
            try
            {
                _logger.LogInformation("Invoking downstream API to create range statistic for Employee ID {EmployeeID}.", dto.EmployeeID);
                return await _downstreamApi.CallApiForUserAsync<RangeStatisticDTO, RangeStatisticDTO>(
                    "MyCustomApi",
                    dto,
                    options =>
                    {
                        options.HttpMethod = HttpMethod.Post.Method;
                        options.RelativePath = "api/rangestatistics";
                    });
            }
            catch (MicrosoftIdentityWebChallengeUserException ex)
            {
                _consentHandler.HandleException(ex);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating range statistic via downstream API.");
                throw;
            }
        }

        public async Task<RangeStatisticDTO?> UpdateRangeStatisticAsync(int id, RangeStatisticDTO dto)
        {
            try
            {
                _logger.LogInformation("Invoking downstream API to update range statistic ID {RangeID}.", id);
                return await _downstreamApi.CallApiForUserAsync<RangeStatisticDTO, RangeStatisticDTO>(
                    "MyCustomApi",
                    dto,
                    options =>
                    {
                        options.HttpMethod = HttpMethod.Put.Method;
                        options.RelativePath = $"api/rangestatistics/{id}";
                    });
            }
            catch (MicrosoftIdentityWebChallengeUserException ex)
            {
                _consentHandler.HandleException(ex);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating range statistic via downstream API.");
                throw;
            }
        }

        public async Task<bool> DeleteRangeStatisticAsync(int id)
        {
            try
            {
                _logger.LogInformation("Invoking downstream API to delete range statistic ID {RangeID}.", id);
                var response = await _downstreamApi.CallApiForUserAsync(
                    "MyCustomApi",
                    options =>
                    {
                        options.HttpMethod = HttpMethod.Delete.Method;
                        options.RelativePath = $"api/rangestatistics/{id}";
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
                _logger.LogError(ex, "Error deleting range statistic via downstream API.");
                return false;
            }
        }

        public async Task<IEnumerable<RangeCourseDTO>> GetRangeCoursesAsync()
        {
            return await GetCachedOrFetchAsync("SearchService_RangeCourses", async () =>
            {
                try
                {
                    var response = await _downstreamApi.CallApiForUserAsync<IEnumerable<RangeCourseDTO>>(
                        "MyCustomApi",
                        options =>
                        {
                            options.HttpMethod = HttpMethod.Get.Method;
                            options.RelativePath = "api/rangestatistics/courses";
                        });
                    return response ?? Array.Empty<RangeCourseDTO>();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error fetching range courses via downstream API.");
                    return Array.Empty<RangeCourseDTO>();
                }
            });
        }

        public async Task<IEnumerable<RangeEventDTO>> GetRangeEventsAsync()
        {
            return await GetCachedOrFetchAsync("SearchService_RangeEvents", async () =>
            {
                try
                {
                    var response = await _downstreamApi.CallApiForUserAsync<IEnumerable<RangeEventDTO>>(
                        "MyCustomApi",
                        options =>
                        {
                            options.HttpMethod = HttpMethod.Get.Method;
                            options.RelativePath = "api/rangestatistics/events";
                        });
                    return response ?? Array.Empty<RangeEventDTO>();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error fetching range events via downstream API.");
                    return Array.Empty<RangeEventDTO>();
                }
            });
        }

        public async Task<RangeEventDTO?> CreateRangeEventAsync(RangeEventDTO dto)
        {
            try
            {
                _logger.LogInformation("Invoking downstream API to create range event.");
                return await _downstreamApi.CallApiForUserAsync<RangeEventDTO, RangeEventDTO>(
                    "MyCustomApi",
                    dto,
                    options =>
                    {
                        options.HttpMethod = HttpMethod.Post.Method;
                        options.RelativePath = "api/rangestatistics/events";
                    });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating range event via downstream API.");
                return null;
            }
        }

        public async Task<IEnumerable<EmployeeTrainingsDto>> GetEmployeeTrainingsByEmployeeIdAsync(int employeeId)
        {
            try
            {
                _logger.LogInformation("Invoking downstream API for employee trainings for Employee ID {EmployeeID}.", employeeId);

                var response = await _downstreamApi.CallApiForUserAsync<IEnumerable<EmployeeTrainingsDto>>(
                    "MyCustomApi",
                    options =>
                    {
                        options.HttpMethod = HttpMethod.Get.Method;
                        options.RelativePath = $"api/training/employee/{employeeId}";
                    });

                return response ?? Array.Empty<EmployeeTrainingsDto>();
            }
            catch (MicrosoftIdentityWebChallengeUserException ex)
            {
                _consentHandler.HandleException(ex);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calling downstream API for employee trainings.");
                return Array.Empty<EmployeeTrainingsDto>();
            }
        }

        public async Task<EmployeeTrainingsDto?> CreateEmployeeTrainingAsync(EmployeeTrainingsDto dto)
        {
            try
            {
                _logger.LogInformation("Invoking downstream API to create training record for Employee ID {EmployeeID}.", dto.EmployeeID);
                return await _downstreamApi.CallApiForUserAsync<EmployeeTrainingsDto, EmployeeTrainingsDto>(
                    "MyCustomApi",
                    dto,
                    options =>
                    {
                        options.HttpMethod = HttpMethod.Post.Method;
                        options.RelativePath = "api/training";
                    });
            }
            catch (MicrosoftIdentityWebChallengeUserException ex)
            {
                _consentHandler.HandleException(ex);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating training record via downstream API.");
                throw;
            }
        }

        public async Task<EmployeeTrainingsDto?> UpdateEmployeeTrainingAsync(int id, EmployeeTrainingsDto dto)
        {
            try
            {
                _logger.LogInformation("Invoking downstream API to update training record ID {EmployeeTrainingID}.", id);
                return await _downstreamApi.CallApiForUserAsync<EmployeeTrainingsDto, EmployeeTrainingsDto>(
                    "MyCustomApi",
                    dto,
                    options =>
                    {
                        options.HttpMethod = HttpMethod.Put.Method;
                        options.RelativePath = $"api/training/{id}";
                    });
            }
            catch (MicrosoftIdentityWebChallengeUserException ex)
            {
                _consentHandler.HandleException(ex);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating training record via downstream API.");
                throw;
            }
        }

        public async Task<bool> DeleteEmployeeTrainingAsync(int id)
        {
            try
            {
                _logger.LogInformation("Invoking downstream API to delete training record ID {EmployeeTrainingID}.", id);
                var response = await _downstreamApi.CallApiForUserAsync(
                    "MyCustomApi",
                    options =>
                    {
                        options.HttpMethod = HttpMethod.Delete.Method;
                        options.RelativePath = $"api/training/{id}";
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
                _logger.LogError(ex, "Error deleting training record via downstream API.");
                return false;
            }
        }

        public async Task<IEnumerable<TrainingExpirationDto>> GetTrainingExpirationsAsync(DateOnly? startDate, DateOnly? endDate)
        {
            try
            {
                var queryParams = new List<string>();
                if (startDate.HasValue) queryParams.Add($"startDate={startDate.Value:yyyy-MM-dd}");
                if (endDate.HasValue) queryParams.Add($"endDate={endDate.Value:yyyy-MM-dd}");

                var relativePath = "api/training/expirations";
                if (queryParams.Any())
                {
                    relativePath += "?" + string.Join("&", queryParams);
                }

                _logger.LogInformation("Invoking downstream API for training expirations.");

                var response = await _downstreamApi.CallApiForUserAsync<IEnumerable<TrainingExpirationDto>>(
                    "MyCustomApi",
                    options =>
                    {
                        options.HttpMethod = HttpMethod.Get.Method;
                        options.RelativePath = relativePath;
                    });

                return response ?? Array.Empty<TrainingExpirationDto>();
            }
            catch (MicrosoftIdentityWebChallengeUserException ex)
            {
                _consentHandler.HandleException(ex);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calling downstream API for training expirations.");
                return Array.Empty<TrainingExpirationDto>();
            }
        }

        public async Task<IEnumerable<TrainingBySchoolDto>> GetTrainingBySchoolAsync(TrainingBySchoolRequestDto request)
        {
            try
            {
                _logger.LogInformation("Invoking downstream API for training by school report.");

                var response = await _downstreamApi.CallApiForUserAsync<TrainingBySchoolRequestDto, IEnumerable<TrainingBySchoolDto>>(
                    "MyCustomApi",
                    request,
                    options =>
                    {
                        options.HttpMethod = HttpMethod.Post.Method;
                        options.RelativePath = "api/training/by-school";
                    });

                return response ?? Array.Empty<TrainingBySchoolDto>();
            }
            catch (MicrosoftIdentityWebChallengeUserException ex)
            {
                _consentHandler.HandleException(ex);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calling downstream API for training by school report.");
                return Array.Empty<TrainingBySchoolDto>();
            }
        }

        public async Task<IEnumerable<TrainingSessionsDto>> GetTrainingSessionsAsync()
        {
            try
            {
                var response = await _downstreamApi.CallApiForUserAsync<IEnumerable<TrainingSessionsDto>>(
                    "MyCustomApi",
                    options =>
                    {
                        options.HttpMethod = HttpMethod.Get.Method;
                        options.RelativePath = "api/training/sessions";
                    });
                return response ?? Array.Empty<TrainingSessionsDto>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching training sessions via downstream API.");
                return Array.Empty<TrainingSessionsDto>();
            }
        }

        public async Task<TrainingSessionsDto?> CreateTrainingSessionAsync(TrainingSessionsDto dto)
        {
            try
            {
                _logger.LogInformation("Invoking downstream API to create training session.");
                return await _downstreamApi.CallApiForUserAsync<TrainingSessionsDto, TrainingSessionsDto>(
                    "MyCustomApi",
                    dto,
                    options =>
                    {
                        options.HttpMethod = HttpMethod.Post.Method;
                        options.RelativePath = "api/training/sessions";
                    });
            }
            catch (MicrosoftIdentityWebChallengeUserException ex)
            {
                _consentHandler.HandleException(ex);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating training session via downstream API.");
                throw;
            }
        }

        public async Task<TrainingSessionsDto?> UpdateTrainingSessionAsync(int id, TrainingSessionsDto dto)
        {
            try
            {
                _logger.LogInformation("Invoking downstream API to update training session ID {SessionID}.", id);
                return await _downstreamApi.CallApiForUserAsync<TrainingSessionsDto, TrainingSessionsDto>(
                    "MyCustomApi",
                    dto,
                    options =>
                    {
                        options.HttpMethod = HttpMethod.Put.Method;
                        options.RelativePath = $"api/training/sessions/{id}";
                    });
            }
            catch (MicrosoftIdentityWebChallengeUserException ex)
            {
                _consentHandler.HandleException(ex);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating training session via downstream API.");
                throw;
            }
        }

        public async Task<IEnumerable<TrainingSchoolsDto>> GetTrainingSchoolsAsync()
        {
            try
            {
                var response = await _downstreamApi.CallApiForUserAsync<IEnumerable<TrainingSchoolsDto>>(
                    "MyCustomApi",
                    options =>
                    {
                        options.HttpMethod = HttpMethod.Get.Method;
                        options.RelativePath = "api/training/schools";
                    });
                return response ?? Array.Empty<TrainingSchoolsDto>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching training schools via downstream API.");
                return Array.Empty<TrainingSchoolsDto>();
            }
        }

        public async Task<TrainingSchoolsDto?> CreateTrainingSchoolAsync(TrainingSchoolsDto dto)
        {
            try
            {
                _logger.LogInformation("Invoking downstream API to create new training school.");
                return await _downstreamApi.CallApiForUserAsync<TrainingSchoolsDto, TrainingSchoolsDto>(
                    "MyCustomApi",
                    dto,
                    options =>
                    {
                        options.HttpMethod = HttpMethod.Post.Method;
                        options.RelativePath = "api/trainingschools";
                    });
            }
            catch (MicrosoftIdentityWebChallengeUserException ex)
            {
                _consentHandler.HandleException(ex);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating training school via downstream API.");
                throw;
            }
        }

        public async Task<IEnumerable<EmployeeSearchResultDto>> GetActiveEmployeesAsync()
        {
            try
            {
                _logger.LogInformation("Invoking downstream API for active employees.");
                var response = await _downstreamApi.CallApiForUserAsync<IEnumerable<EmployeeSearchResultDto>>(
                    "MyCustomApi",
                    options =>
                    {
                        options.HttpMethod = HttpMethod.Get.Method;
                        options.RelativePath = "api/employee/search/active";
                    });

                return response ?? Array.Empty<EmployeeSearchResultDto>();
            }
            catch (MicrosoftIdentityWebChallengeUserException ex)
            {
                _consentHandler.HandleException(ex);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get active employees from downstream API.");
                return Array.Empty<EmployeeSearchResultDto>();
            }
        }

        public async Task<MultipleTrainingAssignmentResultDto?> AssignMultipleTrainingsAsync(MultipleTrainingAssignmentRequestDto request)
        {
            try
            {
                _logger.LogInformation("Invoking downstream API to assign training session to multiple employees.");
                return await _downstreamApi.CallApiForUserAsync<MultipleTrainingAssignmentRequestDto, MultipleTrainingAssignmentResultDto>(
                    "MyCustomApi",
                    request,
                    options =>
                    {
                        options.HttpMethod = HttpMethod.Post.Method;
                        options.RelativePath = "api/training/assign-multiple";
                    });
            }
            catch (MicrosoftIdentityWebChallengeUserException ex)
            {
                _consentHandler.HandleException(ex);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to assign multiple trainings via downstream API.");
                throw;
            }
        }

        public async Task<MultipleEquipmentAssignmentResultDto?> AssignMultipleEquipmentAsync(MultipleEquipmentAssignmentRequestDto request)
        {
            try
            {
                _logger.LogInformation("Invoking downstream API to assign multiple equipment items to multiple employees.");
                return await _downstreamApi.CallApiForUserAsync<MultipleEquipmentAssignmentRequestDto, MultipleEquipmentAssignmentResultDto>(
                    "MyCustomApi",
                    request,
                    options =>
                    {
                        options.HttpMethod = HttpMethod.Post.Method;
                        options.RelativePath = "api/inventory/assign-multiple";
                    });
            }
            catch (MicrosoftIdentityWebChallengeUserException ex)
            {
                _consentHandler.HandleException(ex);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to assign multiple equipment items via downstream API.");
                throw;
            }
        }

        public async Task<IEnumerable<EmployeeDocumentDto>> GetEmployeeDocumentsByEmployeeIdAsync(int employeeId)
        {
            try
            {
                _logger.LogInformation("Invoking downstream API for employee documents.");
                var response = await _downstreamApi.CallApiForUserAsync<IEnumerable<EmployeeDocumentDto>>(
                    "MyCustomApi",
                    options =>
                    {
                        options.HttpMethod = HttpMethod.Get.Method;
                        options.RelativePath = $"api/documents/employee/{employeeId}";
                    });

                return response ?? Array.Empty<EmployeeDocumentDto>();
            }
            catch (MicrosoftIdentityWebChallengeUserException ex)
            {
                _consentHandler.HandleException(ex);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get documents for employee ID {EmployeeID}.", employeeId);
                return Array.Empty<EmployeeDocumentDto>();
            }
        }

        public async Task<EmployeeDocumentDto?> GetDocumentByIdAsync(int id)
        {
            try
            {
                _logger.LogInformation("Invoking downstream API to get document ID {DocumentID}.", id);
                return await _downstreamApi.CallApiForUserAsync<EmployeeDocumentDto>(
                    "MyCustomApi",
                    options =>
                    {
                        options.HttpMethod = HttpMethod.Get.Method;
                        options.RelativePath = $"api/documents/{id}";
                    });
            }
            catch (MicrosoftIdentityWebChallengeUserException ex)
            {
                _consentHandler.HandleException(ex);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get document ID {DocumentID}.", id);
                return null;
            }
        }

        public async Task<IEnumerable<DocumentTypeDto>> GetDocumentTypesAsync()
        {
            return await GetCachedOrFetchAsync("SearchService_DocumentTypes", async () =>
            {
                try
                {
                    _logger.LogInformation("Invoking downstream API for document types.");
                    var response = await _downstreamApi.CallApiForUserAsync<IEnumerable<DocumentTypeDto>>(
                        "MyCustomApi",
                        options =>
                        {
                            options.HttpMethod = HttpMethod.Get.Method;
                            options.RelativePath = "api/documents/types";
                        });

                    return response ?? Array.Empty<DocumentTypeDto>();
                }
                catch (MicrosoftIdentityWebChallengeUserException ex)
                {
                    _consentHandler.HandleException(ex);
                    throw;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to get document types.");
                    return Array.Empty<DocumentTypeDto>();
                }
            });
        }

        public async Task<EmployeeDocumentDto?> UploadDocumentAsync(EmployeeDocumentDto dto)
        {
            try
            {
                _logger.LogInformation("Invoking downstream API to upload document for employee ID {EmployeeID}.", dto.EmployeeID);
                return await _downstreamApi.CallApiForUserAsync<EmployeeDocumentDto, EmployeeDocumentDto>(
                    "MyCustomApi",
                    dto,
                    options =>
                    {
                        options.HttpMethod = HttpMethod.Post.Method;
                        options.RelativePath = "api/documents";
                    });
            }
            catch (MicrosoftIdentityWebChallengeUserException ex)
            {
                _consentHandler.HandleException(ex);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to upload document via downstream API.");
                throw;
            }
        }

        public async Task<EmployeeDocumentDto?> UpdateDocumentAsync(EmployeeDocumentDto dto)
        {
            try
            {
                _logger.LogInformation("Invoking downstream API to update document ID {DocumentID}.", dto.DocumentID);
                return await _downstreamApi.CallApiForUserAsync<EmployeeDocumentDto, EmployeeDocumentDto>(
                    "MyCustomApi",
                    dto,
                    options =>
                    {
                        options.HttpMethod = HttpMethod.Put.Method;
                        options.RelativePath = $"api/documents/{dto.DocumentID}";
                    });
            }
            catch (MicrosoftIdentityWebChallengeUserException ex)
            {
                _consentHandler.HandleException(ex);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update document ID {DocumentID} via downstream API.", dto.DocumentID);
                throw;
            }
        }

        public async Task<bool> DeleteDocumentAsync(int id)
        {
            try
            {
                _logger.LogInformation("Invoking downstream API to delete document ID {DocumentID}.", id);
                var response = await _downstreamApi.CallApiForUserAsync(
                    "MyCustomApi",
                    options =>
                    {
                        options.HttpMethod = HttpMethod.Delete.Method;
                        options.RelativePath = $"api/documents/{id}";
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
                _logger.LogError(ex, "Failed to delete document ID {DocumentID}.", id);
                return false;
            }
        }
    }
}