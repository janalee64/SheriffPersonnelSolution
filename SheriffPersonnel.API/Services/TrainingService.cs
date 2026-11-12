using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SheriffPersonnel.API.Repositories;
using SheriffPersonnel.Shared.DTOs;
using SheriffPersonnel.Shared.Models;

namespace SheriffPersonnel.API.Services
{
    public class TrainingService : ITrainingService
    {
        private readonly ITrainingRepository _repository;
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IUserRepository _userRepository;

        public TrainingService(
            ITrainingRepository repository,
            IEmployeeRepository employeeRepository,
            IUserRepository userRepository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _employeeRepository = employeeRepository ?? throw new ArgumentNullException(nameof(employeeRepository));
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        }

        private async Task<int?> ResolveOrCreateTrainingSchoolIdAsync(int? schoolId, string? schoolName)
        {
            if (schoolId.HasValue && schoolId.Value > 0)
            {
                return schoolId.Value;
            }

            if (string.IsNullOrWhiteSpace(schoolName))
            {
                return null;
            }

            var trimmedName = schoolName.Trim();
            var schools = await _repository.GetTrainingSchoolsAsync();
            var existingSchool = schools.FirstOrDefault(s => string.Equals(s.SchoolName?.Trim(), trimmedName, StringComparison.OrdinalIgnoreCase));
            if (existingSchool != null)
            {
                return existingSchool.TrainingSchoolID;
            }

            var newSchool = new TrainingSchool
            {
                SchoolName = trimmedName,
                IsActive = true
            };

            var createdSchool = await _repository.AddTrainingSchoolAsync(newSchool);
            return createdSchool.TrainingSchoolID;
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

        public async Task<IEnumerable<EmployeeTrainingsDto>> GetEmployeeTrainingsByEmployeeIdAsync(int employeeId)
        {
            var entities = await _repository.GetEmployeeTrainingsByEmployeeIdAsync(employeeId);
            return entities.Select(MapToDto);
        }

        public async Task<EmployeeTrainingsDto?> GetEmployeeTrainingByIdAsync(int id)
        {
            var entity = await _repository.GetEmployeeTrainingByIdAsync(id);
            return entity == null ? null : MapToDto(entity);
        }

        public async Task<EmployeeTrainingsDto> CreateEmployeeTrainingAsync(EmployeeTrainingsDto dto)
        {
            if (dto == null) throw new ArgumentNullException(nameof(dto));

            if (dto.TrainingSessionID <= 0)
            {
                if (!dto.BeginDate.HasValue)
                {
                    throw new ArgumentException("Start Date is required when creating a training session.", nameof(dto.BeginDate));
                }
                if (!dto.EndDate.HasValue)
                {
                    throw new ArgumentException("End Date is required when creating a training session.", nameof(dto.EndDate));
                }
                if (dto.BeginDate.Value > dto.EndDate.Value)
                {
                    throw new ArgumentException("End Date cannot be earlier than Start Date.", nameof(dto.EndDate));
                }

                int? schoolId = await ResolveOrCreateTrainingSchoolIdAsync(dto.TrainingSchoolID, dto.SchoolName);
                var session = new TrainingSession
                {
                    TrainingSchoolID = schoolId,
                    Location = dto.Location,
                    Instructor = dto.Instructor,
                    BeginDate = dto.BeginDate,
                    EndDate = dto.EndDate,
                    SessionHours = dto.SessionHours
                };
                var createdSession = await _repository.AddTrainingSessionAsync(session);
                dto.TrainingSessionID = createdSession.TrainingSessionID;
            }

            int validUserId = await ResolveValidUserIdAsync(dto.LastUserID);

            var entity = new EmployeeTraining
            {
                EmployeeID = dto.EmployeeID,
                TrainingSessionID = dto.TrainingSessionID,
                HoursCompleted = dto.HoursCompleted,
                ExpireDate = dto.ExpireDate,
                OrderNumber = dto.OrderNumber,
                RegistrationCost = dto.RegistrationCost,
                TravelCost = dto.TravelCost,
                LastUserID = validUserId,
                LastUpdated = DateTime.Now
            };

            var created = await _repository.AddEmployeeTrainingAsync(entity);
            var reloaded = await _repository.GetEmployeeTrainingByIdAsync(created.EmployeeTrainingID);
            return MapToDto(reloaded ?? created);
        }

        public async Task<EmployeeTrainingsDto?> UpdateEmployeeTrainingAsync(int id, EmployeeTrainingsDto dto)
        {
            if (dto == null) throw new ArgumentNullException(nameof(dto));

            var existing = await _repository.GetEmployeeTrainingByIdAsync(id);
            if (existing == null) return null;

            if (dto.TrainingSessionID <= 0)
            {
                if (!dto.BeginDate.HasValue)
                {
                    throw new ArgumentException("Start Date is required when creating a training session.", nameof(dto.BeginDate));
                }
                if (!dto.EndDate.HasValue)
                {
                    throw new ArgumentException("End Date is required when creating a training session.", nameof(dto.EndDate));
                }
                if (dto.BeginDate.Value > dto.EndDate.Value)
                {
                    throw new ArgumentException("End Date cannot be earlier than Start Date.", nameof(dto.EndDate));
                }

                int? schoolId = await ResolveOrCreateTrainingSchoolIdAsync(dto.TrainingSchoolID, dto.SchoolName);
                var session = new TrainingSession
                {
                    TrainingSchoolID = schoolId,
                    Location = dto.Location,
                    Instructor = dto.Instructor,
                    BeginDate = dto.BeginDate,
                    EndDate = dto.EndDate,
                    SessionHours = dto.SessionHours
                };
                var createdSession = await _repository.AddTrainingSessionAsync(session);
                dto.TrainingSessionID = createdSession.TrainingSessionID;
            }

            existing.TrainingSessionID = dto.TrainingSessionID;
            existing.HoursCompleted = dto.HoursCompleted;
            existing.ExpireDate = dto.ExpireDate;
            existing.OrderNumber = dto.OrderNumber;
            existing.RegistrationCost = dto.RegistrationCost;
            existing.TravelCost = dto.TravelCost;
            existing.LastUserID = await ResolveValidUserIdAsync(dto.LastUserID > 0 ? dto.LastUserID : existing.LastUserID);
            existing.LastUpdated = DateTime.Now;

            await _repository.UpdateEmployeeTrainingAsync(existing);
            var reloaded = await _repository.GetEmployeeTrainingByIdAsync(id);
            return MapToDto(reloaded ?? existing);
        }

        public async Task<bool> DeleteEmployeeTrainingAsync(int id)
        {
            return await _repository.DeleteEmployeeTrainingAsync(id);
        }

        public async Task<IEnumerable<TrainingExpirationDto>> GetTrainingExpirationsAsync(DateOnly startDate, DateOnly endDate)
        {
            var entities = await _repository.GetTrainingExpirationsAsync(startDate, endDate);
            return entities.Select(et =>
            {
                var emp = et.Employee;
                var school = et.TrainingSession?.TrainingSchool;

                var nameParts = new List<string>();
                if (!string.IsNullOrWhiteSpace(emp?.LastName)) nameParts.Add(emp.LastName.Trim());
                if (!string.IsNullOrWhiteSpace(emp?.FirstName)) nameParts.Add(emp.FirstName.Trim());
                if (!string.IsNullOrWhiteSpace(emp?.MiddleInitial)) nameParts.Add(emp.MiddleInitial.Trim());

                return new TrainingExpirationDto
                {
                    EmployeeTrainingID = et.EmployeeTrainingID,
                    EmployeeID = et.EmployeeID,
                    ExpireDate = et.ExpireDate,
                    SchoolName = school?.SchoolName ?? string.Empty,
                    FirstName = emp?.FirstName,
                    MiddleName = emp?.MiddleInitial,
                    LastName = emp?.LastName,
                    EmployeeName = string.Join(", ", nameParts)
                };
            });
        }

        public async Task<IEnumerable<TrainingSessionsDto>> GetTrainingSessionsAsync()
        {
            var sessions = await _repository.GetTrainingSessionsAsync();
            return sessions.Select(s => new TrainingSessionsDto
            {
                TrainingSessionID = s.TrainingSessionID,
                TrainingSchoolID = s.TrainingSchoolID,
                SchoolName = s.TrainingSchool?.SchoolName,
                Location = s.Location,
                Instructor = s.Instructor,
                BeginDate = s.BeginDate,
                EndDate = s.EndDate,
                SessionHours = s.SessionHours
            });
        }

        public async Task<TrainingSessionsDto> CreateTrainingSessionAsync(TrainingSessionsDto dto)
        {
            if (dto == null) throw new ArgumentNullException(nameof(dto));

            int? schoolId = await ResolveOrCreateTrainingSchoolIdAsync(dto.TrainingSchoolID, dto.SchoolName);

            var entity = new TrainingSession
            {
                TrainingSchoolID = schoolId,
                Location = dto.Location,
                Instructor = dto.Instructor,
                BeginDate = dto.BeginDate,
                EndDate = dto.EndDate,
                SessionHours = dto.SessionHours
            };

            var created = await _repository.AddTrainingSessionAsync(entity);
            var reloaded = await _repository.GetTrainingSessionByIdAsync(created.TrainingSessionID);
            return new TrainingSessionsDto
            {
                TrainingSessionID = (reloaded ?? created).TrainingSessionID,
                TrainingSchoolID = (reloaded ?? created).TrainingSchoolID,
                SchoolName = (reloaded ?? created).TrainingSchool?.SchoolName,
                Location = (reloaded ?? created).Location,
                Instructor = (reloaded ?? created).Instructor,
                BeginDate = (reloaded ?? created).BeginDate,
                EndDate = (reloaded ?? created).EndDate,
                SessionHours = (reloaded ?? created).SessionHours
            };
        }

        public async Task<TrainingSessionsDto?> UpdateTrainingSessionAsync(int id, TrainingSessionsDto dto)
        {
            if (dto == null) throw new ArgumentNullException(nameof(dto));

            var existing = await _repository.GetTrainingSessionByIdAsync(id);
            if (existing == null) return null;

            int? schoolId = await ResolveOrCreateTrainingSchoolIdAsync(dto.TrainingSchoolID, dto.SchoolName);

            existing.TrainingSchoolID = schoolId;
            existing.Location = dto.Location;
            existing.Instructor = dto.Instructor;
            existing.BeginDate = dto.BeginDate;
            existing.EndDate = dto.EndDate;
            existing.SessionHours = dto.SessionHours;

            await _repository.UpdateTrainingSessionAsync(existing);
            var reloaded = await _repository.GetTrainingSessionByIdAsync(id);
            return new TrainingSessionsDto
            {
                TrainingSessionID = (reloaded ?? existing).TrainingSessionID,
                TrainingSchoolID = (reloaded ?? existing).TrainingSchoolID,
                SchoolName = (reloaded ?? existing).TrainingSchool?.SchoolName,
                Location = (reloaded ?? existing).Location,
                Instructor = (reloaded ?? existing).Instructor,
                BeginDate = (reloaded ?? existing).BeginDate,
                EndDate = (reloaded ?? existing).EndDate,
                SessionHours = (reloaded ?? existing).SessionHours
            };
        }

        public async Task<IEnumerable<TrainingSchoolsDto>> GetTrainingSchoolsAsync()
        {
            var schools = await _repository.GetTrainingSchoolsAsync();
            return schools.Select(s => new TrainingSchoolsDto
            {
                TrainingSchoolID = s.TrainingSchoolID,
                SchoolName = s.SchoolName,
                Address = s.Address,
                IsActive = s.IsActive ?? true
            });
        }

        public async Task<MultipleTrainingAssignmentResultDto> AssignMultipleTrainingsAsync(MultipleTrainingAssignmentRequestDto request)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));

            var result = new MultipleTrainingAssignmentResultDto
            {
                TotalRequested = request.EmployeeIDs?.Count ?? 0
            };

            if (request.EmployeeIDs == null || !request.EmployeeIDs.Any())
            {
                return result;
            }

            int sessionId = request.TrainingSessionID;
            TrainingSession? session = null;

            if (sessionId <= 0)
            {
                session = new TrainingSession
                {
                    TrainingSchoolID = request.TrainingSchoolID,
                    Location = request.Location,
                    Instructor = request.Instructor,
                    BeginDate = request.BeginDate,
                    EndDate = request.EndDate,
                    SessionHours = request.SessionHours
                };
                session = await _repository.AddTrainingSessionAsync(session);
                sessionId = session.TrainingSessionID;
            }
            else
            {
                session = await _repository.GetTrainingSessionByIdAsync(sessionId);
            }

            result.TrainingSessionID = sessionId;

            int validUserId = await ResolveValidUserIdAsync(request.LastUserID);

            DateOnly? newBegin = session?.BeginDate ?? request.BeginDate;
            DateOnly? newEnd = session?.EndDate ?? request.EndDate;

            DateOnly? targetStart = newBegin ?? newEnd;
            DateOnly? targetEnd = newEnd ?? newBegin;
            if (targetStart.HasValue && targetEnd.HasValue && targetStart.Value > targetEnd.Value)
            {
                (targetStart, targetEnd) = (targetEnd, targetStart);
            }

            foreach (var empId in request.EmployeeIDs.Distinct())
            {
                var employee = await _employeeRepository.GetEmployeeByIdAsync(empId);
                string empName = employee != null ? $"{employee.FirstName} {employee.LastName}".Trim() : $"Employee #{empId}";
                string? badge = employee?.BadgeNumber?.ToString();

                var existingTrainings = await _repository.GetEmployeeTrainingsByEmployeeIdAsync(empId);

                bool hasConflict = false;
                EmployeeTrainingConflictDto? conflictDto = null;

                if (targetStart.HasValue && targetEnd.HasValue)
                {
                    foreach (var et in existingTrainings)
                    {
                        var existSession = et.TrainingSession;
                        if (existSession == null) continue;

                        DateOnly? existBegin = existSession.BeginDate;
                        DateOnly? existEnd = existSession.EndDate;

                        DateOnly? existStart = existBegin ?? existEnd;
                        DateOnly? existE = existEnd ?? existBegin;

                        if (existStart.HasValue && existE.HasValue)
                        {
                            if (existStart.Value > existE.Value)
                            {
                                (existStart, existE) = (existE, existStart);
                            }

                            if (targetStart.Value <= existE.Value && existStart.Value <= targetEnd.Value)
                            {
                                hasConflict = true;
                                string schoolName = existSession.TrainingSchool?.SchoolName ?? existSession.Location ?? "Training Session";
                                string dateStr = (existBegin.HasValue || existEnd.HasValue)
                                    ? $"({existBegin:MM/dd/yyyy} - {existEnd:MM/dd/yyyy})"
                                    : "";

                                conflictDto = new EmployeeTrainingConflictDto
                                {
                                    EmployeeID = empId,
                                    EmployeeName = empName,
                                    BadgeNumber = badge,
                                    ConflictingTrainingSessionID = existSession.TrainingSessionID,
                                    ConflictingSchoolOrLocation = schoolName,
                                    ConflictingBeginDate = existBegin,
                                    ConflictingEndDate = existEnd,
                                    ConflictMessage = $"{empName}{(string.IsNullOrWhiteSpace(badge) ? "" : $" (Badge #{badge})")} already has training '{schoolName}' scheduled {dateStr}."
                                };
                                break;
                            }
                        }
                    }
                }

                if (hasConflict && conflictDto != null)
                {
                    result.Conflicts.Add(conflictDto);
                }
                else
                {
                    var entity = new EmployeeTraining
                    {
                        EmployeeID = empId,
                        TrainingSessionID = sessionId,
                        HoursCompleted = request.HoursCompleted,
                        ExpireDate = request.ExpireDate,
                        OrderNumber = request.OrderNumber,
                        RegistrationCost = request.RegistrationCost,
                        TravelCost = request.TravelCost,
                        LastUserID = validUserId,
                        LastUpdated = DateTime.Now
                    };

                    await _repository.AddEmployeeTrainingAsync(entity);
                    result.AssignedEmployeeIDs.Add(empId);
                }
            }

            return result;
        }

        public async Task<IEnumerable<TrainingBySchoolDto>> GetTrainingBySchoolAsync(TrainingBySchoolRequestDto request)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));

            var entities = await _repository.GetTrainingBySchoolAsync(request);

            var dtos = entities.Select(et =>
            {
                var emp = et.Employee;
                var session = et.TrainingSession;
                var school = session?.TrainingSchool;

                var latestProgram = emp?.EmployeeChanges?
                    .Where(ec => ec.Program != null && !string.IsNullOrWhiteSpace(ec.Program.ProgramName))
                    .OrderByDescending(ec => ec.HistoryDate)
                    .ThenByDescending(ec => ec.HistoryID)
                    .Select(ec => ec.Program!.ProgramName)
                    .FirstOrDefault() ?? string.Empty;

                var latestUnit = emp?.EmployeeChanges?
                    .Where(ec => ec.Unit != null && !string.IsNullOrWhiteSpace(ec.Unit.UnitName))
                    .OrderByDescending(ec => ec.HistoryDate)
                    .ThenByDescending(ec => ec.HistoryID)
                    .Select(ec => ec.Unit!.UnitName)
                    .FirstOrDefault() ?? string.Empty;

                var nameParts = new List<string>();
                if (!string.IsNullOrWhiteSpace(emp?.LastName)) nameParts.Add(emp.LastName.Trim());
                if (!string.IsNullOrWhiteSpace(emp?.FirstName)) nameParts.Add(emp.FirstName.Trim());
                var formattedName = nameParts.Count > 0 ? string.Join(", ", nameParts) : (emp != null ? $"Employee #{emp.EmployeeID}" : "Unknown");

                return new TrainingBySchoolDto
                {
                    EmployeeTrainingID = et.EmployeeTrainingID,
                    EmployeeID = et.EmployeeID,
                    EmployeeName = formattedName,
                    EmployeeStatus = emp?.Status?.StatusDescription ?? emp?.Status?.StatusCode ?? string.Empty,
                    Program = latestProgram,
                    UnitName = latestUnit,
                    BeginDate = session?.BeginDate,
                    Hours = et.HoursCompleted ?? session?.SessionHours,
                    SchoolName = school?.SchoolName ?? request.SchoolName ?? string.Empty
                };
            });

            return dtos.OrderBy(x => x.EmployeeName, StringComparer.OrdinalIgnoreCase);
        }

        private static EmployeeTrainingsDto MapToDto(EmployeeTraining entity)
        {
            return new EmployeeTrainingsDto
            {
                EmployeeTrainingID = entity.EmployeeTrainingID,
                EmployeeID = entity.EmployeeID,
                TrainingSessionID = entity.TrainingSessionID,
                HoursCompleted = entity.HoursCompleted,
                ExpireDate = entity.ExpireDate,
                OrderNumber = entity.OrderNumber,
                RegistrationCost = entity.RegistrationCost,
                TravelCost = entity.TravelCost,
                LastUserID = entity.LastUserID,
                LastUpdated = entity.LastUpdated,
                TrainingSchoolID = entity.TrainingSession?.TrainingSchoolID,
                SchoolName = entity.TrainingSession?.TrainingSchool?.SchoolName,
                Location = entity.TrainingSession?.Location,
                Instructor = entity.TrainingSession?.Instructor,
                BeginDate = entity.TrainingSession?.BeginDate,
                EndDate = entity.TrainingSession?.EndDate,
                SessionHours = entity.TrainingSession?.SessionHours
            };
        }
    }
}
