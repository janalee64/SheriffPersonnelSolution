using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SheriffPersonnel.API.Repositories;
using SheriffPersonnel.Shared.Data;
using SheriffPersonnel.Shared.DTOs;
using SheriffPersonnel.Shared.Models;

namespace SheriffPersonnel.API.Services
{
    public class RangeStatisticService : IRangeStatisticService
    {
        private readonly IRangeStatisticRepository _repository;
        private readonly IUserRepository _userRepository;
        private readonly DBContext _dbContext;
        private readonly ILogger<RangeStatisticService> _logger;

        public RangeStatisticService(
            IRangeStatisticRepository repository,
            IUserRepository userRepository,
            DBContext dbContext,
            ILogger<RangeStatisticService> logger)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        private async Task<int> ResolveValidUserIdAsync(int userId)
        {
            if (userId > 0)
            {
                var existingUser = await _userRepository.GetUserByIdAsync(userId);
                if (existingUser != null)
                {
                    return userId;
                }
            }

            var users = await _userRepository.GetAllUsersAsync();
            var firstUser = users.FirstOrDefault();
            if (firstUser != null)
            {
                return firstUser.UserID;
            }

            return 1;
        }

        public async Task<IEnumerable<RangeStatisticDTO>> GetAllRangeStatisticsAsync()
        {
            try
            {
                var list = await _repository.GetAllRangeStatisticsAsync();
                return list.Select(MapToDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all range statistics.");
                throw;
            }
        }

        public async Task<IEnumerable<RangeStatisticDTO>> GetRangeStatisticsByEmployeeIdAsync(int employeeId)
        {
            try
            {
                var list = await _repository.GetRangeStatisticsByEmployeeIdAsync(employeeId);
                return list.Select(MapToDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting range statistics for employee ID {EmployeeID}.", employeeId);
                throw;
            }
        }

        public async Task<RangeStatisticDTO?> GetRangeStatisticByIdAsync(int id)
        {
            try
            {
                var entity = await _repository.GetRangeStatisticByIdAsync(id);
                return entity != null ? MapToDto(entity) : null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting range statistic ID {RangeID}.", id);
                throw;
            }
        }

        public async Task<RangeStatisticDTO> CreateRangeStatisticAsync(RangeStatisticDTO dto)
        {
            try
            {
                if (dto == null) throw new ArgumentNullException(nameof(dto));

                int validUserId = await ResolveValidUserIdAsync(dto.LastUserID);

                var entity = MapToEntity(dto);
                entity.LastUserID = validUserId;

                if (dto.SelectedSkillIDs != null && dto.SelectedSkillIDs.Any())
                {
                    var skills = await _dbContext.RangeSkills
                        .Where(s => dto.SelectedSkillIDs.Contains(s.RangeSkillID))
                        .ToListAsync();
                    entity.RangeSkills = skills;
                }

                var created = await _repository.AddRangeStatisticAsync(entity);

                var reloaded = await _repository.GetRangeStatisticByIdAsync(created.RangeID) ?? created;
                return MapToDto(reloaded);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating range statistic record.");
                throw;
            }
        }

        public async Task<RangeStatisticDTO?> UpdateRangeStatisticAsync(int id, RangeStatisticDTO dto)
        {
            try
            {
                if (dto == null) throw new ArgumentNullException(nameof(dto));

                var existing = await _repository.GetRangeStatisticByIdAsync(id);
                if (existing == null)
                {
                    _logger.LogWarning("Range statistic ID {RangeID} not found for update.", id);
                    return null;
                }

                int validUserId = await ResolveValidUserIdAsync(dto.LastUserID > 0 ? dto.LastUserID : existing.LastUserID);

                existing.EmployeeID = dto.EmployeeID;
                existing.FirearmID = dto.FirearmID;
                existing.CourseID = dto.CourseID;
                existing.RoundsFired = dto.RoundsFired;
                existing.PassFail = ToDbPassFail(dto.PassFail);
                existing.LastUserID = validUserId;
                existing.RangeEventID = dto.RangeEventID;

                existing.RangeSkills.Clear();
                if (dto.SelectedSkillIDs != null && dto.SelectedSkillIDs.Any())
                {
                    var skills = await _dbContext.RangeSkills
                        .Where(s => dto.SelectedSkillIDs.Contains(s.RangeSkillID))
                        .ToListAsync();
                    foreach (var skill in skills)
                    {
                        existing.RangeSkills.Add(skill);
                    }
                }

                var updated = await _repository.UpdateRangeStatisticAsync(existing);
                var reloaded = await _repository.GetRangeStatisticByIdAsync(updated.RangeID) ?? updated;
                return MapToDto(reloaded);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating range statistic ID {RangeID}.", id);
                throw;
            }
        }

        public async Task<bool> DeleteRangeStatisticAsync(int id)
        {
            try
            {
                return await _repository.DeleteRangeStatisticAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting range statistic ID {RangeID}.", id);
                throw;
            }
        }

        public async Task<IEnumerable<RangeCourseDTO>> GetRangeCoursesAsync()
        {
            try
            {
                var courses = await _repository.GetRangeCoursesAsync();
                return courses.Select(c => new RangeCourseDTO
                {
                    CourseID = c.CourseID,
                    CourseCode = c.CourseCode,
                    SortOrder = c.SortOrder,
                    CourseName = c.CourseName,
                    Notes = c.Notes,
                    IsActive = c.IsActive ?? true
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting range courses.");
                throw;
            }
        }

        public async Task<IEnumerable<RangeEventDTO>> GetRangeEventsAsync()
        {
            try
            {
                var events = await _repository.GetRangeEventsAsync();
                return events.Select(e => new RangeEventDTO
                {
                    RangeEventID = e.RangeEventID,
                    RangeDate = e.RangeDate,
                    Location = e.Location,
                    EventType = e.EventType,
                    Notes = e.Notes,
                    RangeOfficer = e.RangeOfficer
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting range events.");
                throw;
            }
        }

        public async Task<RangeEventDTO> CreateRangeEventAsync(RangeEventDTO dto)
        {
            try
            {
                if (dto == null) throw new ArgumentNullException(nameof(dto));
                var entity = new RangeEvent
                {
                    RangeDate = dto.RangeDate,
                    Location = dto.Location,
                    EventType = dto.EventType,
                    Notes = dto.Notes,
                    RangeOfficerID = dto.RangeOfficer?.RangeOfficerID > 0 ? dto.RangeOfficer.RangeOfficerID : null,
                    LastUpdated = DateTime.Now
                };
                var created = await _repository.AddRangeEventAsync(entity);
                return new RangeEventDTO
                {
                    RangeEventID = created.RangeEventID,
                    RangeDate = created.RangeDate,
                    Location = created.Location,
                    EventType = created.EventType,
                    Notes = created.Notes
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating range event.");
                throw;
            }
        }

        private static string? ToDbPassFail(string? value)
        {
            if (string.IsNullOrWhiteSpace(value)) return value;
            if (string.Equals(value, "Pass", StringComparison.OrdinalIgnoreCase) || string.Equals(value, "P", StringComparison.OrdinalIgnoreCase))
                return "P";
            if (string.Equals(value, "Fail", StringComparison.OrdinalIgnoreCase) || string.Equals(value, "F", StringComparison.OrdinalIgnoreCase))
                return "F";
            return value;
        }

        private static string? ToDisplayPassFail(string? value)
        {
            if (string.IsNullOrWhiteSpace(value)) return value;
            if (string.Equals(value, "P", StringComparison.OrdinalIgnoreCase) || string.Equals(value, "Pass", StringComparison.OrdinalIgnoreCase))
                return "Pass";
            if (string.Equals(value, "F", StringComparison.OrdinalIgnoreCase) || string.Equals(value, "Fail", StringComparison.OrdinalIgnoreCase))
                return "Fail";
            return value;
        }

        private static RangeStatisticDTO MapToDto(RangeStatistic entity)
        {
            string? firearmDesc = null;
            string? serialNumber = null;
            if (entity.Firearm != null)
            {
                var make = entity.Firearm.Make?.MakeName;
                var model = entity.Firearm.ModelNumber;
                serialNumber = entity.Firearm.SerialNumber;
                firearmDesc = string.Join(" ", new[] { make, model, !string.IsNullOrEmpty(serialNumber) ? $"({serialNumber})" : null }.Where(s => !string.IsNullOrWhiteSpace(s)));
            }

            string? empName = null;
            string? empStatus = null;
            int? empStatusId = null;
            string? latestProgram = null;
            int? latestProgramId = null;
            string? latestUnit = null;
            int? latestUnitId = null;

            if (entity.Employee != null)
            {
                empName = $"{entity.Employee.FirstName} {entity.Employee.LastName}".Trim();
                empStatus = entity.Employee.Status?.StatusDescription ?? entity.Employee.Status?.StatusCode;
                empStatusId = entity.Employee.StatusID;

                if (entity.Employee.EmployeeChanges != null && entity.Employee.EmployeeChanges.Count > 0)
                {
                    var latestProgramChange = entity.Employee.EmployeeChanges
                        .Where(ec => ec.Program != null && !string.IsNullOrWhiteSpace(ec.Program.ProgramName))
                        .OrderByDescending(ec => ec.HistoryDate)
                        .ThenByDescending(ec => ec.HistoryID)
                        .FirstOrDefault();

                    if (latestProgramChange != null)
                    {
                        latestProgram = latestProgramChange.Program?.ProgramName;
                        latestProgramId = latestProgramChange.ProgramID;
                    }

                    var latestUnitChange = entity.Employee.EmployeeChanges
                        .Where(ec => ec.Unit != null && !string.IsNullOrWhiteSpace(ec.Unit.UnitName))
                        .OrderByDescending(ec => ec.HistoryDate)
                        .ThenByDescending(ec => ec.HistoryID)
                        .FirstOrDefault();

                    if (latestUnitChange != null)
                    {
                        latestUnit = latestUnitChange.Unit?.UnitName;
                        latestUnitId = latestUnitChange.UnitID;
                    }
                }
            }

            string? eventDesc = null;
            if (entity.RangeEvent != null)
            {
                eventDesc = string.Join(" - ", new[] { entity.RangeEvent.EventType, entity.RangeEvent.Location }.Where(s => !string.IsNullOrWhiteSpace(s)));
            }

            return new RangeStatisticDTO
            {
                RangeID = entity.RangeID,
                EmployeeID = entity.EmployeeID,
                EmployeeName = empName,
                EmployeeStatus = empStatus,
                EmployeeStatusID = empStatusId,
                Program = latestProgram,
                ProgramID = latestProgramId,
                Unit = latestUnit,
                UnitID = latestUnitId,
                FirearmID = entity.FirearmID,
                FirearmDescription = firearmDesc,
                SerialNumber = serialNumber,
                CourseID = entity.CourseID,
                CourseCode = entity.Course?.CourseCode,
                CourseName = entity.Course?.CourseName,
                RoundsFired = entity.RoundsFired,
                PassFail = ToDisplayPassFail(entity.PassFail),
                LastUserID = entity.LastUserID,
                LastUserName = entity.LastUser?.DisplayName ?? entity.LastUser?.UserPrincipalName,
                LastUpdated = entity.LastUpdated,
                RangeEventID = entity.RangeEventID,
                RangeDate = entity.RangeEvent?.RangeDate,
                EventDescription = eventDesc,
                SelectedSkillIDs = entity.RangeSkills != null ? entity.RangeSkills.Select(s => s.RangeSkillID).ToList() : new List<int>()
            };
        }

        private static RangeStatistic MapToEntity(RangeStatisticDTO dto)
        {
            return new RangeStatistic
            {
                RangeID = dto.RangeID,
                EmployeeID = dto.EmployeeID,
                FirearmID = dto.FirearmID,
                CourseID = dto.CourseID,
                RoundsFired = dto.RoundsFired,
                PassFail = ToDbPassFail(dto.PassFail),
                LastUserID = dto.LastUserID,
                RangeEventID = dto.RangeEventID
            };
        }
    }
}

