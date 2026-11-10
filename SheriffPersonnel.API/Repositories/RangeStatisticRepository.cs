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
            return await _dbContext.RangeStatistics
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
                .OrderByDescending(rs => rs.RangeEvent != null ? rs.RangeEvent.RangeDate : DateOnly.MinValue)
                .ThenBy(rs => rs.Employee != null ? rs.Employee.LastName : "")
                .ToListAsync();
        }

        public async Task<IEnumerable<RangeStatistic>> GetRangeStatisticsByEmployeeIdAsync(int employeeId)
        {
            return await _dbContext.RangeStatistics
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
                .ToListAsync();
        }

        public async Task<RangeStatistic?> GetRangeStatisticByIdAsync(int id)
        {
            return await _dbContext.RangeStatistics
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
                .OrderBy(c => c.SortOrder ?? int.MaxValue)
                .ThenBy(c => c.CourseName)
                .ToListAsync();
        }

        public async Task<IEnumerable<RangeEvent>> GetRangeEventsAsync()
        {
            return await _dbContext.RangeEvents
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

