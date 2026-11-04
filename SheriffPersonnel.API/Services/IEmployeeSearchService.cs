using System.Collections.Generic;
using System.Threading.Tasks;
using SheriffPersonnel.Shared.DTOs;

namespace SheriffPersonnel.API.Services
{
    public interface IEmployeeSearchService
    {
        Task<IEnumerable<EmployeeSearchResultDto>> SearchEmployeesAsync(EmployeeSearchRequestDto request);
        Task<IEnumerable<EmployeeSearchResultDto>> GetActiveEmployeesAsync();
        Task<IEnumerable<StatusesDto>> GetStatusesAsync();
        Task<EmployeeDetailDto?> GetEmployeeByIdAsync(int id);
        Task<EmployeeDetailDto> CreateEmployeeAsync(EmployeeDetailDto dto);
        Task<EmployeeDetailDto?> UpdateEmployeeAsync(int id, EmployeeDetailDto dto);
        Task<bool> DeleteEmployeeAsync(int id);
    }
}
