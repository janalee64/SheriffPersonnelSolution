using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SheriffPersonnel.API.Repositories;
using SheriffPersonnel.Shared.DTOs;
using SheriffPersonnel.Shared.Models;
using ProgramModel = SheriffPersonnel.Shared.Models.Program;

namespace SheriffPersonnel.API.Services
{
    public class EmployeeChangeService : IEmployeeChangeService
    {
        private readonly IEmployeeChangeRepository _repository;
        private readonly ILogger<EmployeeChangeService> _logger;

        public EmployeeChangeService(
            IEmployeeChangeRepository repository,
            ILogger<EmployeeChangeService> logger)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<IEnumerable<EmployeeChangesDto>> GetChangesByEmployeeIdAsync(int employeeId)
        {
            try
            {
                var changes = await _repository.GetChangesByEmployeeIdAsync(employeeId);
                return changes.Select(MapToDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting employee changes for employee ID {EmployeeID}.", employeeId);
                throw;
            }
        }

        public async Task<EmployeeChangesDto?> GetChangeByIdAsync(int id)
        {
            try
            {
                var change = await _repository.GetChangeByIdAsync(id);
                return change != null ? MapToDto(change) : null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting employee change ID {HistoryID}.", id);
                throw;
            }
        }

        public async Task<EmployeeChangesDto> CreateChangeAsync(EmployeeChangesDto dto)
        {
            try
            {
                if (dto == null) throw new ArgumentNullException(nameof(dto));

                var entity = MapToEntity(dto);
                var created = await _repository.AddChangeAsync(entity);

                var reloaded = await _repository.GetChangeByIdAsync(created.HistoryID) ?? created;
                return MapToDto(reloaded);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating employee change record.");
                throw;
            }
        }

        public async Task<EmployeeChangesDto?> UpdateChangeAsync(int id, EmployeeChangesDto dto)
        {
            try
            {
                if (dto == null) throw new ArgumentNullException(nameof(dto));

                var existing = await _repository.GetChangeByIdAsync(id);
                if (existing == null)
                {
                    _logger.LogWarning("Employee change ID {HistoryID} not found for update.", id);
                    return null;
                }

                existing.HistoryDate = dto.HistoryDate;
                existing.ProgramID = dto.ProgramID;
                existing.RankID = dto.RankID;
                existing.UnitID = dto.UnitID;
                existing.LastUserID = dto.LastUserID;

                var updated = await _repository.UpdateChangeAsync(existing);
                var reloaded = await _repository.GetChangeByIdAsync(updated.HistoryID) ?? updated;
                return MapToDto(reloaded);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating employee change ID {HistoryID}.", id);
                throw;
            }
        }

        public async Task<bool> DeleteChangeAsync(int id)
        {
            try
            {
                return await _repository.DeleteChangeAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting employee change ID {HistoryID}.", id);
                throw;
            }
        }

        public async Task<IEnumerable<ProgramsDto>> GetProgramsAsync()
        {
            try
            {
                var programs = await _repository.GetProgramsAsync();
                return programs.Select(p => new ProgramsDto
                {
                    ProgramID = p.ProgramID,
                    ProgramName = p.ProgramName,
                    Description = p.Description,
                    IsActive = p.IsActive ?? true
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting programs list.");
                throw;
            }
        }

        public async Task<IEnumerable<RanksDto>> GetRanksAsync()
        {
            try
            {
                var ranks = await _repository.GetRanksAsync();
                return ranks.Select(r => new RanksDto
                {
                    RankID = r.RankID,
                    RankName = r.RankName,
                    IsActive = r.IsActive ?? true
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting ranks list.");
                throw;
            }
        }

        public async Task<IEnumerable<UnitsDto>> GetUnitsAsync()
        {
            try
            {
                var units = await _repository.GetUnitsAsync();
                return units.Select(u => new UnitsDto
                {
                    UnitID = u.UnitID,
                    UnitName = u.UnitName,
                    Notes = u.Notes,
                    IsActive = u.IsActive ?? true
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting units list.");
                throw;
            }
        }

        private static EmployeeChangesDto MapToDto(EmployeeChange entity)
        {
            return new EmployeeChangesDto
            {
                HistoryID = entity.HistoryID,
                EmployeeID = entity.EmployeeID,
                HistoryDate = entity.HistoryDate,
                ProgramID = entity.ProgramID,
                ProgramName = entity.Program?.ProgramName,
                RankID = entity.RankID,
                RankName = entity.Rank?.RankName,
                UnitID = entity.UnitID,
                UnitName = entity.Unit?.UnitName,
                LastUserID = entity.LastUserID,
                LastUserName = entity.LastUser?.DisplayName ?? entity.LastUser?.UserPrincipalName,
                LastUpdated = entity.LastUpdated
            };
        }

        private static EmployeeChange MapToEntity(EmployeeChangesDto dto)
        {
            return new EmployeeChange
            {
                HistoryID = dto.HistoryID,
                EmployeeID = dto.EmployeeID,
                HistoryDate = dto.HistoryDate,
                ProgramID = dto.ProgramID,
                RankID = dto.RankID,
                UnitID = dto.UnitID,
                LastUserID = dto.LastUserID
            };
        }
    }
}

