using System.Collections.Generic;
using System.Threading.Tasks;
using SheriffPersonnel.Shared.DTOs;

namespace SheriffPersonnel.API.Services
{
    public interface IRangeStatisticService
    {
        Task<IEnumerable<RangeStatisticDTO>> GetAllRangeStatisticsAsync();
        Task<IEnumerable<RangeStatisticDTO>> GetRangeStatisticsByEmployeeIdAsync(int employeeId);
        Task<RangeStatisticDTO?> GetRangeStatisticByIdAsync(int id);
        Task<RangeStatisticDTO> CreateRangeStatisticAsync(RangeStatisticDTO dto);
        Task<RangeStatisticDTO?> UpdateRangeStatisticAsync(int id, RangeStatisticDTO dto);
        Task<bool> DeleteRangeStatisticAsync(int id);
        Task<IEnumerable<RangeCourseDTO>> GetRangeCoursesAsync();
        Task<IEnumerable<RangeEventDTO>> GetRangeEventsAsync();
        Task<RangeEventDTO> CreateRangeEventAsync(RangeEventDTO dto);
    }
}

