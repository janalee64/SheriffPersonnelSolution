using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SheriffPersonnel.Shared.Models;

namespace SheriffPersonnel.API.Repositories
{
    public interface IRangeStatisticRepository
    {
        Task<IEnumerable<RangeStatistic>> GetAllRangeStatisticsAsync();
        Task<IEnumerable<RangeStatistic>> GetRangeStatisticsFilteredAsync(
            int? courseId = null,
            int? employeeStatusId = null,
            int? programId = null,
            IEnumerable<int>? unitIds = null,
            DateOnly? startDate = null,
            DateOnly? endDate = null,
            string? passFail = null);
        Task<IEnumerable<RangeStatistic>> GetRangeStatisticsByEmployeeIdAsync(int employeeId);
        Task<RangeStatistic?> GetRangeStatisticByIdAsync(int id);
        Task<RangeStatistic> AddRangeStatisticAsync(RangeStatistic entity);
        Task<RangeStatistic> UpdateRangeStatisticAsync(RangeStatistic entity);
        Task<bool> DeleteRangeStatisticAsync(int id);
        Task<IEnumerable<RangeCourse>> GetRangeCoursesAsync();
        Task<IEnumerable<RangeEvent>> GetRangeEventsAsync();
        Task<RangeEvent> AddRangeEventAsync(RangeEvent entity);
    }
}

