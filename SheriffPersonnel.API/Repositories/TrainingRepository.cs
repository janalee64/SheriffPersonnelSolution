using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SheriffPersonnel.Shared.Data;
using SheriffPersonnel.Shared.DTOs;
using SheriffPersonnel.Shared.Models;

namespace SheriffPersonnel.API.Repositories
{
    public class TrainingRepository : ITrainingRepository
    {
        private readonly DBContext _dbContext;

        public TrainingRepository(DBContext dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public async Task<IEnumerable<EmployeeTraining>> GetEmployeeTrainingsByEmployeeIdAsync(int employeeId)
        {
            return await _dbContext.EmployeeTrainings
                .AsNoTracking()
                .Include(et => et.TrainingSession)
                    .ThenInclude(s => s!.TrainingSchool)
                .Where(et => et.EmployeeID == employeeId)
                .OrderByDescending(et => et.TrainingSession != null ? et.TrainingSession.BeginDate : DateOnly.MinValue)
                .ThenByDescending(et => et.EmployeeTrainingID)
                .ToListAsync();
        }

        public async Task<EmployeeTraining?> GetEmployeeTrainingByIdAsync(int id)
        {
            return await _dbContext.EmployeeTrainings
                .AsNoTracking()
                .Include(et => et.TrainingSession)
                    .ThenInclude(s => s!.TrainingSchool)
                .FirstOrDefaultAsync(et => et.EmployeeTrainingID == id);
        }

        public async Task<EmployeeTraining> AddEmployeeTrainingAsync(EmployeeTraining employeeTraining)
        {
            if (employeeTraining == null) throw new ArgumentNullException(nameof(employeeTraining));

            employeeTraining.LastUpdated = DateTime.Now;
            await _dbContext.EmployeeTrainings.AddAsync(employeeTraining);
            await _dbContext.SaveChangesAsync();
            return employeeTraining;
        }

        public async Task<EmployeeTraining> UpdateEmployeeTrainingAsync(EmployeeTraining employeeTraining)
        {
            if (employeeTraining == null) throw new ArgumentNullException(nameof(employeeTraining));

            employeeTraining.LastUpdated = DateTime.Now;
            var entry = _dbContext.Entry(employeeTraining);
            if (entry.State == EntityState.Detached)
            {
                _dbContext.EmployeeTrainings.Update(employeeTraining);
            }

            await _dbContext.SaveChangesAsync();
            return employeeTraining;
        }

        public async Task<bool> DeleteEmployeeTrainingAsync(int id)
        {
            var record = await _dbContext.EmployeeTrainings.FindAsync(id);
            if (record == null) return false;

            _dbContext.EmployeeTrainings.Remove(record);
            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<EmployeeTraining>> GetTrainingExpirationsAsync(DateOnly startDate, DateOnly endDate)
        {
            return await _dbContext.EmployeeTrainings
                .Include(et => et.Employee)
                .Include(et => et.TrainingSession)
                    .ThenInclude(s => s!.TrainingSchool)
                .Where(et => et.ExpireDate != null && et.ExpireDate >= startDate && et.ExpireDate <= endDate)
                .OrderBy(et => et.ExpireDate)
                .ThenBy(et => et.TrainingSession != null && et.TrainingSession.TrainingSchool != null ? et.TrainingSession.TrainingSchool.SchoolName : "")
                .ThenBy(et => et.Employee != null ? et.Employee.LastName : "")
                .ToListAsync();
        }

        public async Task<IEnumerable<TrainingSession>> GetTrainingSessionsAsync()
        {
            return await _dbContext.TrainingSessions
                .Include(ts => ts.TrainingSchool)
                .OrderByDescending(ts => ts.BeginDate)
                .ToListAsync();
        }

        public async Task<TrainingSession?> GetTrainingSessionByIdAsync(int id)
        {
            return await _dbContext.TrainingSessions
                .Include(ts => ts.TrainingSchool)
                .FirstOrDefaultAsync(ts => ts.TrainingSessionID == id);
        }

        public async Task<TrainingSession> AddTrainingSessionAsync(TrainingSession trainingSession)
        {
            if (trainingSession == null) throw new ArgumentNullException(nameof(trainingSession));

            await _dbContext.TrainingSessions.AddAsync(trainingSession);
            await _dbContext.SaveChangesAsync();
            return trainingSession;
        }

        public async Task<TrainingSession> UpdateTrainingSessionAsync(TrainingSession trainingSession)
        {
            if (trainingSession == null) throw new ArgumentNullException(nameof(trainingSession));

            var entry = _dbContext.Entry(trainingSession);
            if (entry.State == EntityState.Detached)
            {
                _dbContext.TrainingSessions.Update(trainingSession);
            }

            await _dbContext.SaveChangesAsync();
            return trainingSession;
        }

        public async Task<IEnumerable<TrainingSchool>> GetTrainingSchoolsAsync()
        {
            return await _dbContext.TrainingSchools
                .OrderBy(s => s.SchoolName)
                .ToListAsync();
        }

        public async Task<IEnumerable<EmployeeTraining>> GetTrainingBySchoolAsync(TrainingBySchoolRequestDto request)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));

            var query = _dbContext.EmployeeTrainings
                .Include(et => et.Employee)
                    .ThenInclude(e => e!.Status)
                .Include(et => et.Employee)
                    .ThenInclude(e => e!.EmployeeChanges)
                        .ThenInclude(ec => ec.Program)
                .Include(et => et.Employee)
                    .ThenInclude(e => e!.EmployeeChanges)
                        .ThenInclude(ec => ec.Unit)
                .Include(et => et.TrainingSession)
                    .ThenInclude(s => s!.TrainingSchool)
                .AsQueryable();

            if (request.TrainingSchoolID.HasValue && request.TrainingSchoolID.Value > 0)
            {
                var schoolId = request.TrainingSchoolID.Value;
                if (!string.IsNullOrWhiteSpace(request.SchoolName))
                {
                    var schoolName = request.SchoolName.Trim();
                    query = query.Where(et => et.TrainingSession != null && et.TrainingSession.TrainingSchool != null &&
                        (et.TrainingSession.TrainingSchoolID == schoolId ||
                         et.TrainingSession.TrainingSchool.SchoolName == schoolName ||
                         EF.Functions.Like(et.TrainingSession.TrainingSchool.SchoolName, $"%{schoolName}%")));
                }
                else
                {
                    query = query.Where(et => et.TrainingSession != null && et.TrainingSession.TrainingSchoolID == schoolId);
                }
            }
            else if (!string.IsNullOrWhiteSpace(request.SchoolName))
            {
                var schoolName = request.SchoolName.Trim();
                query = query.Where(et => et.TrainingSession != null && et.TrainingSession.TrainingSchool != null &&
                    (et.TrainingSession.TrainingSchool.SchoolName == schoolName ||
                     EF.Functions.Like(et.TrainingSession.TrainingSchool.SchoolName, $"%{schoolName}%")));
            }

            if (request.StatusID.HasValue && request.StatusID.Value > 0)
            {
                query = query.Where(et => et.Employee != null && et.Employee.StatusID == request.StatusID.Value);
            }

            if (request.ProgramID.HasValue && request.ProgramID.Value > 0)
            {
                query = query.Where(et => et.Employee != null && et.Employee.EmployeeChanges.Any(ec => ec.ProgramID == request.ProgramID.Value));
            }

            if (request.UnitID.HasValue && request.UnitID.Value > 0)
            {
                query = query.Where(et => et.Employee != null && et.Employee.EmployeeChanges.Any(ec => ec.UnitID == request.UnitID.Value));
            }

            if (request.StartDate.HasValue)
            {
                query = query.Where(et => et.TrainingSession != null && et.TrainingSession.BeginDate != null && et.TrainingSession.BeginDate >= request.StartDate.Value);
            }

            if (request.EndDate.HasValue)
            {
                query = query.Where(et => et.TrainingSession != null && et.TrainingSession.BeginDate != null && et.TrainingSession.BeginDate <= request.EndDate.Value);
            }

            return await query
                .OrderBy(et => et.Employee != null ? et.Employee.LastName : "")
                .ThenBy(et => et.Employee != null ? et.Employee.FirstName : "")
                .ToListAsync();
        }
    }
}
