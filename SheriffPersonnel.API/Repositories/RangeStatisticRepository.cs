using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SheriffPersonnel.Shared.Data;
using SheriffPersonnel.Shared.Models;

namespace SheriffPersonnel.API.Repositories
{
    public class RangeStatisticRepository : IRangeStatisticRepository
    {
        private readonly DBContext _dbContext;

        public RangeStatisticRepository(DBContext dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public async Task<IEnumerable<RangeStatistic>> GetAllRangeStatisticsAsync()
        {
            return await GetRangeStatisticsFilteredAsync();
        }

        public async Task<IEnumerable<RangeStatistic>> GetRangeStatisticsFilteredAsync(
            int? courseId = null,
            int? employeeStatusId = null,
            int? programId = null,
            IEnumerable<int>? unitIds = null,
            DateOnly? startDate = null,
            DateOnly? endDate = null,
            string? passFail = null)
        {
            var query = _dbContext.RangeStatistics
                .AsNoTracking()
                .AsSplitQuery()
                .Include(rs => rs.Employee)
                    .ThenInclude(e => e!.Status)
                .Include(rs => rs.Employee)
                    .ThenInclude(e => e!.EmployeeChanges)
                        .ThenInclude(ec => ec.Program)
                .Include(rs => rs.Employee)
                    .ThenInclude(e => e!.EmployeeChanges)
                        .ThenInclude(ec => ec.Unit)
                .Include(rs => rs.Firearm)
                    .ThenInclude(f => f!.Make)
                .Include(rs => rs.Course)
                .Include(rs => rs.RangeEvent)
                .Include(rs => rs.LastUser)
                .Include(rs => rs.RangeSkills)
                .AsQueryable();

            if (courseId.HasValue && courseId.Value > 0)
            {
                query = query.Where(rs => rs.CourseID == courseId.Value);
            }

            if (employeeStatusId.HasValue && employeeStatusId.Value > 0)
            {
                query = query.Where(rs => rs.Employee != null && rs.Employee.StatusID == employeeStatusId.Value);
            }

            if (startDate.HasValue)
            {
                query = query.Where(rs => rs.RangeEvent != null && rs.RangeEvent.RangeDate >= startDate.Value);
            }

            if (endDate.HasValue)
            {
                query = query.Where(rs => rs.RangeEvent != null && rs.RangeEvent.RangeDate <= endDate.Value);
            }

            if (!string.IsNullOrWhiteSpace(passFail))
            {
                string normPassFail = passFail.Trim();
                if (string.Equals(normPassFail, "Pass", StringComparison.OrdinalIgnoreCase) || string.Equals(normPassFail, "P", StringComparison.OrdinalIgnoreCase))
                {
                    query = query.Where(rs => rs.PassFail == "P" || rs.PassFail == "Pass");
                }
                else if (string.Equals(normPassFail, "Fail", StringComparison.OrdinalIgnoreCase) || string.Equals(normPassFail, "F", StringComparison.OrdinalIgnoreCase))
                {
                    query = query.Where(rs => rs.PassFail == "F" || rs.PassFail == "Fail");
                }
                else
                {
                    query = query.Where(rs => rs.PassFail == normPassFail);
                }
            }

            if (programId.HasValue && programId.Value > 0)
            {
                query = query.Where(rs => rs.Employee != null && rs.Employee.EmployeeChanges
                    .Where(ec => ec.ProgramID != null)
                    .OrderByDescending(ec => ec.HistoryDate)
                    .ThenByDescending(ec => ec.HistoryID)
                    .Select(ec => ec.ProgramID)
                    .FirstOrDefault() == programId.Value);
            }

            var uList = unitIds?.Where(u => u > 0).ToList();
            if (uList != null && uList.Any())
            {
                query = query.Where(rs => rs.Employee != null && uList.Contains(
                    rs.Employee.EmployeeChanges
                        .Where(ec => ec.UnitID != null)
                        .OrderByDescending(ec => ec.HistoryDate)
                        .ThenByDescending(ec => ec.HistoryID)
                        .Select(ec => ec.UnitID!.Value)
                        .FirstOrDefault()));
            }

            return await query
                .OrderByDescending(rs => rs.RangeEvent != null ? rs.RangeEvent.RangeDate : DateOnly.MinValue)
                .ThenBy(rs => rs.Employee != null ? rs.Employee.LastName : "")
                .ToListAsync();
        }

        public async Task<IEnumerable<RangeStatistic>> GetRangeStatisticsByEmployeeIdAsync(int employeeId)
        {
            return await _dbContext.RangeStatistics
                .AsNoTracking()
                .AsSplitQuery()
                .Include(rs => rs.Employee)
                    .ThenInclude(e => e!.Status)
                .Include(rs => rs.Employee)
                    .ThenInclude(e => e!.EmployeeChanges)
                        .ThenInclude(ec => ec.Program)
                .Include(rs => rs.Employee)
                    .ThenInclude(e => e!.EmployeeChanges)
                        .ThenInclude(ec => ec.Unit)
                .Include(rs => rs.Firearm)
                    .ThenInclude(f => f!.Make)
                .Include(rs => rs.Course)
                .Include(rs => rs.RangeEvent)
                .Include(rs => rs.LastUser)
                .Include(rs => rs.RangeSkills)
                .Where(rs => rs.EmployeeID == employeeId)
                .OrderByDescending(rs => rs.RangeEvent != null ? rs.RangeEvent.RangeDate : DateOnly.MinValue)
                .ThenByDescending(rs => rs.RangeID)
                .ToListAsync();
        }

        public async Task<RangeStatistic?> GetRangeStatisticByIdAsync(int id)
        {
            return await _dbContext.RangeStatistics
                .AsNoTracking()
                .Include(rs => rs.Employee)
                    .ThenInclude(e => e!.Status)
                .Include(rs => rs.Employee)
                    .ThenInclude(e => e!.EmployeeChanges)
                        .ThenInclude(ec => ec.Program)
                .Include(rs => rs.Employee)
                    .ThenInclude(e => e!.EmployeeChanges)
                        .ThenInclude(ec => ec.Unit)
                .Include(rs => rs.Firearm)
                    .ThenInclude(f => f!.Make)
                .Include(rs => rs.Course)
                .Include(rs => rs.RangeEvent)
                .Include(rs => rs.LastUser)
                .Include(rs => rs.RangeSkills)
                .FirstOrDefaultAsync(rs => rs.RangeID == id);
        }

        public async Task<RangeStatistic> AddRangeStatisticAsync(RangeStatistic entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            entity.LastUpdated = DateTime.Now;
            await _dbContext.RangeStatistics.AddAsync(entity);
            await _dbContext.SaveChangesAsync();
            return entity;
        }

        public async Task<RangeStatistic> UpdateRangeStatisticAsync(RangeStatistic entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            entity.LastUpdated = DateTime.Now;

            var entry = _dbContext.Entry(entity);
            if (entry.State == EntityState.Detached)
            {
                _dbContext.RangeStatistics.Update(entity);
            }

            await _dbContext.SaveChangesAsync();
            return entity;
        }

        public async Task<bool> DeleteRangeStatisticAsync(int id)
        {
            var entity = await _dbContext.RangeStatistics.FindAsync(id);
            if (entity == null) return false;

            _dbContext.RangeStatistics.Remove(entity);
            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<RangeCourse>> GetRangeCoursesAsync()
        {
            return await _dbContext.RangeCourses
                .AsNoTracking()
                .OrderBy(c => c.SortOrder ?? int.MaxValue)
                .ThenBy(c => c.CourseName)
                .ToListAsync();
        }

        public async Task<IEnumerable<RangeEvent>> GetRangeEventsAsync()
        {
            return await _dbContext.RangeEvents
                .AsNoTracking()
                .Include(e => e.RangeOfficer)
                .OrderByDescending(e => e.RangeDate)
                .ToListAsync();
        }

        public async Task<RangeEvent> AddRangeEventAsync(RangeEvent entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));
            entity.LastUpdated = DateTime.Now;
            await _dbContext.RangeEvents.AddAsync(entity);
            await _dbContext.SaveChangesAsync();
            return entity;
        }
    }
}

