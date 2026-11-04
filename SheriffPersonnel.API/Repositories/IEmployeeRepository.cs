using System.Collections.Generic;
using System.Threading.Tasks;
using SheriffPersonnel.Shared.DTOs;
using SheriffPersonnel.Shared.Models;

namespace SheriffPersonnel.API.Repositories
{
    public interface IEmployeeRepository
    {
        Task<IEnumerable<Employee>> SearchEmployeesAsync(EmployeeSearchRequestDto request);
        Task<IEnumerable<Employee>> GetActiveEmployeesAsync();
        Task<IEnumerable<Status>> GetStatusesAsync();
        Task<Employee?> GetEmployeeByIdAsync(int id);
        Task<Employee> AddEmployeeAsync(Employee employee);
        Task<Employee> UpdateEmployeeAsync(Employee employee);
        Task<bool> DeleteEmployeeAsync(int id);
    }
}
