using System.Collections.Generic;
using System.Threading.Tasks;
using SheriffPersonnel.Shared.DTOs;

namespace SheriffPersonnel.API.Services
{
    public interface IEmployeeChangeService
    {
        Task<IEnumerable<EmployeeChangesDto>> GetChangesByEmployeeIdAsync(int employeeId);
        Task<EmployeeChangesDto?> GetChangeByIdAsync(int id);
        Task<EmployeeChangesDto> CreateChangeAsync(EmployeeChangesDto dto);
        Task<EmployeeChangesDto?> UpdateChangeAsync(int id, EmployeeChangesDto dto);
        Task<bool> DeleteChangeAsync(int id);

        Task<IEnumerable<ProgramsDto>> GetProgramsAsync();
        Task<IEnumerable<RanksDto>> GetRanksAsync();
        Task<IEnumerable<UnitsDto>> GetUnitsAsync();
    }
}

