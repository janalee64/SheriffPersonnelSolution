using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SheriffPersonnel.API.Repositories;
using SheriffPersonnel.Shared.DTOs;

namespace SheriffPersonnel.API.Services
{
    public class EmployeeSearchService : IEmployeeSearchService
    {
        private readonly IEmployeeRepository _repository;
        private readonly Microsoft.Extensions.Logging.ILogger<EmployeeSearchService> _logger;

        public EmployeeSearchService(
            IEmployeeRepository repository,
            Microsoft.Extensions.Logging.ILogger<EmployeeSearchService> logger)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<IEnumerable<EmployeeSearchResultDto>> SearchEmployeesAsync(EmployeeSearchRequestDto request)
        {
            try
            {
                var employees = await _repository.SearchEmployeesAsync(request);
                var list = employees.ToList();

                _logger.LogInformation("Employee search completed successfully. Returned {Count} records.", list.Count);

                return list.Select(e =>
                {
                    var latestProgram = e.EmployeeChanges?
                        .Where(ec => ec.Program != null && !string.IsNullOrEmpty(ec.Program.ProgramName))
                        .OrderByDescending(ec => ec.HistoryDate)
                        .ThenByDescending(ec => ec.HistoryID)
                        .Select(ec => ec.Program!.ProgramName)
                        .FirstOrDefault() ?? string.Empty;

                    var latestUnit = e.EmployeeChanges?
                        .Where(ec => ec.Unit != null && !string.IsNullOrEmpty(ec.Unit.UnitName))
                        .OrderByDescending(ec => ec.HistoryDate)
                        .ThenByDescending(ec => ec.HistoryID)
                        .Select(ec => ec.Unit!.UnitName)
                        .FirstOrDefault() ?? string.Empty;

                    return new EmployeeSearchResultDto
                    {
                        EmployeeID = e.EmployeeID,
                        FirstName = e.FirstName ?? string.Empty,
                        LastName = e.LastName ?? string.Empty,
                        BadgeNumber = e.BadgeNumber?.ToString() ?? string.Empty,
                        Status = e.Status?.StatusDescription ?? e.Status?.StatusCode ?? string.Empty,
                        EmployeeFlag = e.EmployeeFlag,
                        EmployeeImage = e.EmployeeImage,
                        HireDate = e.HireDate.HasValue ? e.HireDate.Value.ToDateTime(TimeOnly.MinValue) : null,
                        InactiveDate = e.InactiveDate.HasValue ? e.InactiveDate.Value.ToDateTime(TimeOnly.MinValue) : null,
                        Program = latestProgram,
                        Unit = latestUnit
                    };
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to execute SearchEmployeesAsync in repository.");
                throw;
            }
        }

        public async Task<IEnumerable<EmployeeSearchResultDto>> GetActiveEmployeesAsync()
        {
            try
            {
                var employees = await _repository.GetActiveEmployeesAsync();
                var list = employees.ToList();

                _logger.LogInformation("GetActiveEmployeesAsync completed successfully. Returned {Count} records.", list.Count);

                return list.Select(e =>
                {
                    var latestProgram = e.EmployeeChanges?
                        .Where(ec => ec.Program != null && !string.IsNullOrEmpty(ec.Program.ProgramName))
                        .OrderByDescending(ec => ec.HistoryDate)
                        .ThenByDescending(ec => ec.HistoryID)
                        .Select(ec => ec.Program!.ProgramName)
                        .FirstOrDefault() ?? string.Empty;

                    var latestUnit = e.EmployeeChanges?
                        .Where(ec => ec.Unit != null && !string.IsNullOrEmpty(ec.Unit.UnitName))
                        .OrderByDescending(ec => ec.HistoryDate)
                        .ThenByDescending(ec => ec.HistoryID)
                        .Select(ec => ec.Unit!.UnitName)
                        .FirstOrDefault() ?? string.Empty;

                    return new EmployeeSearchResultDto
                    {
                        EmployeeID = e.EmployeeID,
                        FirstName = e.FirstName ?? string.Empty,
                        LastName = e.LastName ?? string.Empty,
                        BadgeNumber = e.BadgeNumber?.ToString() ?? string.Empty,
                        Status = e.Status?.StatusDescription ?? e.Status?.StatusCode ?? string.Empty,
                        EmployeeFlag = e.EmployeeFlag,
                        EmployeeImage = e.EmployeeImage,
                        HireDate = e.HireDate.HasValue ? e.HireDate.Value.ToDateTime(TimeOnly.MinValue) : null,
                        InactiveDate = e.InactiveDate.HasValue ? e.InactiveDate.Value.ToDateTime(TimeOnly.MinValue) : null,
                        Program = latestProgram,
                        Unit = latestUnit
                    };
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to execute GetActiveEmployeesAsync in repository.");
                throw;
            }
        }

        public async Task<IEnumerable<StatusesDto>> GetStatusesAsync()
        {
            try
            {
                var statuses = await _repository.GetStatusesAsync();
                var list = statuses.ToList();

                _logger.LogInformation("GetStatusesAsync completed successfully. Returned {Count} status records.", list.Count);

                return list.Select(s => new StatusesDto
                {
                    StatusID = s.StatusID,
                    StatusCode = s.StatusCode,
                    StatusDescription = s.StatusDescription
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to execute GetStatusesAsync in repository.");
                throw;
            }
        }

        public async Task<EmployeeDetailDto?> GetEmployeeByIdAsync(int id)
        {
            try
            {
                var employee = await _repository.GetEmployeeByIdAsync(id);
                if (employee == null) return null;

                return MapToDetailDto(employee);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get employee details for ID {EmployeeID}.", id);
                throw;
            }
        }

        public async Task<EmployeeDetailDto> CreateEmployeeAsync(EmployeeDetailDto dto)
        {
            try
            {
                if (dto == null) throw new ArgumentNullException(nameof(dto));

                if (!dto.StatusID.HasValue || dto.StatusID.Value <= 0)
                {
                    var statuses = await _repository.GetStatusesAsync();
                    var activeStatus = statuses.FirstOrDefault(s =>
                        string.Equals(s.StatusCode, "Active", StringComparison.OrdinalIgnoreCase) ||
                        string.Equals(s.StatusDescription, "Active", StringComparison.OrdinalIgnoreCase));
                    if (activeStatus != null)
                    {
                        dto.StatusID = activeStatus.StatusID;
                    }
                }

                var employee = MapToEntity(dto);
                var created = await _repository.AddEmployeeAsync(employee);

                _logger.LogInformation("Successfully created new employee record with ID {EmployeeID}.", created.EmployeeID);

                // Fetch full employee details (with navigation properties)
                var reloaded = await _repository.GetEmployeeByIdAsync(created.EmployeeID) ?? created;
                return MapToDetailDto(reloaded);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create new employee record.");
                throw;
            }
        }

        public async Task<EmployeeDetailDto?> UpdateEmployeeAsync(int id, EmployeeDetailDto dto)
        {
            try
            {
                if (dto == null) throw new ArgumentNullException(nameof(dto));

                var existing = await _repository.GetEmployeeByIdAsync(id);
                if (existing == null)
                {
                    _logger.LogWarning("Cannot update employee ID {EmployeeID} as it was not found.", id);
                    return null;
                }

                existing.FirstName = dto.FirstName;
                existing.MiddleInitial = dto.MiddleInitial;
                existing.LastName = dto.LastName;
                existing.SSN = dto.SSN;
                existing.BadgeNumber = int.TryParse(dto.BadgeNumber, out int bNum) ? bNum : null;
                existing.PTBNumber = dto.PTBNumber;
                existing.CountyEmpNo = dto.CountyEmpNo;
                existing.DOB = dto.DOB.HasValue ? DateOnly.FromDateTime(dto.DOB.Value) : null;
                existing.HireDate = dto.HireDate.HasValue ? DateOnly.FromDateTime(dto.HireDate.Value) : null;
                existing.InactiveDate = dto.InactiveDate.HasValue ? DateOnly.FromDateTime(dto.InactiveDate.Value) : null;
                existing.EmployeeFlag = dto.EmployeeFlag;
                existing.EmployeeImage = dto.EmployeeImage;
                existing.WorkEmail = dto.WorkEmail;
                existing.WorkPhone = dto.WorkPhone;
                existing.HomePhone = dto.HomePhone;
                existing.HomeAddressLine1 = dto.HomeAddressLine1;
                existing.HomeCity = dto.HomeCity;
                existing.HomeState = dto.HomeState;
                existing.HomeZipCode = dto.HomeZipCode;
                existing.StatusID = dto.StatusID;
                existing.Status = null;
                existing.LastUserID = dto.LastUserID;

                var updated = await _repository.UpdateEmployeeAsync(existing);
                _logger.LogInformation("Successfully updated employee record with ID {EmployeeID}.", updated.EmployeeID);

                var reloaded = await _repository.GetEmployeeByIdAsync(updated.EmployeeID) ?? updated;
                return MapToDetailDto(reloaded);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update employee ID {EmployeeID}.", id);
                throw;
            }
        }

        public async Task<bool> DeleteEmployeeAsync(int id)
        {
            try
            {
                var success = await _repository.DeleteEmployeeAsync(id);
                if (success)
                {
                    _logger.LogInformation("Successfully deleted employee record with ID {EmployeeID}.", id);
                }
                return success;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to delete employee ID {EmployeeID}.", id);
                throw;
            }
        }

        private static EmployeeDetailDto MapToDetailDto(SheriffPersonnel.Shared.Models.Employee entity)
        {
            return new EmployeeDetailDto
            {
                EmployeeID = entity.EmployeeID,
                FirstName = entity.FirstName,
                MiddleInitial = entity.MiddleInitial,
                LastName = entity.LastName,
                SSN = entity.SSN,
                BadgeNumber = entity.BadgeNumber?.ToString(),
                PTBNumber = entity.PTBNumber,
                CountyEmpNo = entity.CountyEmpNo,
                DOB = entity.DOB.HasValue ? entity.DOB.Value.ToDateTime(TimeOnly.MinValue) : null,
                HireDate = entity.HireDate.HasValue ? entity.HireDate.Value.ToDateTime(TimeOnly.MinValue) : null,
                InactiveDate = entity.InactiveDate.HasValue ? entity.InactiveDate.Value.ToDateTime(TimeOnly.MinValue) : null,
                EmployeeFlag = entity.EmployeeFlag,
                EmployeeImage = entity.EmployeeImage,
                WorkEmail = entity.WorkEmail,
                WorkPhone = entity.WorkPhone,
                HomePhone = entity.HomePhone,
                HomeAddressLine1 = entity.HomeAddressLine1,
                HomeCity = entity.HomeCity,
                HomeState = entity.HomeState,
                HomeZipCode = entity.HomeZipCode,
                StatusID = entity.StatusID,
                StatusDescription = entity.Status?.StatusDescription ?? entity.Status?.StatusCode,
                LastUserID = entity.LastUserID,
                LastUpdated = entity.LastUpdated
            };
        }

        private static SheriffPersonnel.Shared.Models.Employee MapToEntity(EmployeeDetailDto dto)
        {
            return new SheriffPersonnel.Shared.Models.Employee
            {
                EmployeeID = dto.EmployeeID,
                FirstName = dto.FirstName,
                MiddleInitial = dto.MiddleInitial,
                LastName = dto.LastName,
                SSN = dto.SSN,
                BadgeNumber = int.TryParse(dto.BadgeNumber, out int bNum2) ? bNum2 : null,
                PTBNumber = dto.PTBNumber,
                CountyEmpNo = dto.CountyEmpNo,
                DOB = dto.DOB.HasValue ? DateOnly.FromDateTime(dto.DOB.Value) : null,
                HireDate = dto.HireDate.HasValue ? DateOnly.FromDateTime(dto.HireDate.Value) : null,
                InactiveDate = dto.InactiveDate.HasValue ? DateOnly.FromDateTime(dto.InactiveDate.Value) : null,
                EmployeeFlag = dto.EmployeeFlag,
                EmployeeImage = dto.EmployeeImage,
                WorkEmail = dto.WorkEmail,
                WorkPhone = dto.WorkPhone,
                HomePhone = dto.HomePhone,
                HomeAddressLine1 = dto.HomeAddressLine1,
                HomeCity = dto.HomeCity,
                HomeState = dto.HomeState,
                HomeZipCode = dto.HomeZipCode,
                StatusID = dto.StatusID
            };
        }
    }
}
