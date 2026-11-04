using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SheriffPersonnel.API.Repositories;
using SheriffPersonnel.Shared.DTOs;
using SheriffPersonnel.Shared.Models;

namespace SheriffPersonnel.API.Services
{
    public class FirearmService : IFirearmService
    {
        private readonly IFirearmRepository _repository;
        private readonly ILogger<FirearmService> _logger;

        public FirearmService(
            IFirearmRepository repository,
            ILogger<FirearmService> logger)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<IEnumerable<FirearmsDto>> GetAllFirearmsAsync()
        {
            try
            {
                var firearms = await _repository.GetAllFirearmsAsync();
                return firearms.Select(MapToDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all firearms.");
                throw;
            }
        }

        public async Task<IEnumerable<FirearmsDto>> GetFirearmsByEmployeeIdAsync(int employeeId)
        {
            try
            {
                var firearms = await _repository.GetFirearmsByEmployeeIdAsync(employeeId);
                return firearms.Select(MapToDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting firearms for employee ID {EmployeeID}.", employeeId);
                throw;
            }
        }

        public async Task<FirearmsDto?> GetFirearmByIdAsync(int id)
        {
            try
            {
                var firearm = await _repository.GetFirearmByIdAsync(id);
                return firearm != null ? MapToDto(firearm) : null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting firearm ID {FirearmID}.", id);
                throw;
            }
        }

        public async Task<FirearmsDto> CreateFirearmAsync(FirearmsDto dto)
        {
            try
            {
                if (dto == null) throw new ArgumentNullException(nameof(dto));

                var entity = MapToEntity(dto);
                var created = await _repository.AddFirearmAsync(entity);

                var reloaded = await _repository.GetFirearmByIdAsync(created.FirearmID) ?? created;
                return MapToDto(reloaded);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating firearm record.");
                throw;
            }
        }

        public async Task<FirearmsDto?> UpdateFirearmAsync(int id, FirearmsDto dto)
        {
            try
            {
                if (dto == null) throw new ArgumentNullException(nameof(dto));

                var existing = await _repository.GetFirearmByIdAsync(id);
                if (existing == null)
                {
                    _logger.LogWarning("Firearm ID {FirearmID} not found for update.", id);
                    return null;
                }

                existing.SerialNumber = dto.SerialNumber;
                existing.ModelNumber = dto.ModelNumber;
                existing.Caliber = dto.Caliber;
                existing.EmployeeID = dto.EmployeeID;
                existing.Notes = dto.Notes;
                existing.LastUserID = dto.LastUserID;
                existing.StatusID = dto.StatusID;
                existing.MakeID = dto.MakeID;

                var updated = await _repository.UpdateFirearmAsync(existing);
                var reloaded = await _repository.GetFirearmByIdAsync(updated.FirearmID) ?? updated;
                return MapToDto(reloaded);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating firearm ID {FirearmID}.", id);
                throw;
            }
        }

        public async Task<bool> DeleteFirearmAsync(int id)
        {
            try
            {
                return await _repository.DeleteFirearmAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting firearm ID {FirearmID}.", id);
                throw;
            }
        }

        public async Task<IEnumerable<MakesDto>> GetMakesAsync()
        {
            try
            {
                var makes = await _repository.GetMakesAsync();
                return makes.Select(m => new MakesDto
                {
                    MakeID = m.MakeID,
                    MakeName = m.MakeName
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting makes list.");
                throw;
            }
        }

        private static FirearmsDto MapToDto(Firearm entity)
        {
            string? empName = null;
            string? empStatus = null;
            int? empStatusId = null;
            if (entity.Employee != null)
            {
                empName = $"{entity.Employee.FirstName} {entity.Employee.LastName}".Trim();
                empStatusId = entity.Employee.StatusID;
                empStatus = entity.Employee.Status?.StatusDescription ?? entity.Employee.Status?.StatusCode;
            }

            return new FirearmsDto
            {
                FirearmID = entity.FirearmID,
                SerialNumber = entity.SerialNumber,
                ModelNumber = entity.ModelNumber,
                Caliber = entity.Caliber,
                EmployeeID = entity.EmployeeID,
                EmployeeName = empName,
                EmployeeStatusID = empStatusId,
                EmployeeStatus = empStatus,
                Notes = entity.Notes,
                LastUserID = entity.LastUserID,
                LastUserName = entity.LastUser?.DisplayName ?? entity.LastUser?.UserPrincipalName,
                LastUpdated = entity.LastUpdated,
                StatusID = entity.StatusID,
                StatusDescription = entity.Status?.StatusDescription ?? entity.Status?.StatusCode,
                MakeID = entity.MakeID,
                MakeName = entity.Make?.MakeName
            };
        }

        private static Firearm MapToEntity(FirearmsDto dto)
        {
            return new Firearm
            {
                FirearmID = dto.FirearmID,
                SerialNumber = dto.SerialNumber,
                ModelNumber = dto.ModelNumber,
                Caliber = dto.Caliber,
                EmployeeID = dto.EmployeeID,
                Notes = dto.Notes,
                LastUserID = dto.LastUserID,
                StatusID = dto.StatusID,
                MakeID = dto.MakeID
            };
        }
    }
}

