using System.Collections.Generic;
using System.Threading.Tasks;
using SheriffPersonnel.Shared.Models;

namespace SheriffPersonnel.API.Repositories
{
    public interface IInventoryRepository
    {
        Task<IEnumerable<EmployeeInventoryAssignment>> GetAllAssignmentsAsync();
        Task<IEnumerable<EmployeeInventoryAssignment>> GetAssignmentsByEmployeeIdAsync(int employeeId);
        Task<EmployeeInventoryAssignment?> GetAssignmentByIdAsync(int id);
        Task<Dictionary<int, string>> GetLatestEmployeeProgramsAsync(IEnumerable<int> employeeIds);
        Task<EmployeeInventoryAssignment> AddAssignmentAsync(EmployeeInventoryAssignment assignment);
        Task<int> AddAssignmentsAsync(IEnumerable<EmployeeInventoryAssignment> assignments);
        Task<EmployeeInventoryAssignment> UpdateAssignmentAsync(EmployeeInventoryAssignment assignment);
        Task<bool> DeleteAssignmentAsync(int id);

        Task<IEnumerable<InventoryItem>> GetInventoryItemsAsync(bool? onlyActive = null);
        Task<InventoryItem?> GetInventoryItemByIdAsync(int id);
        Task<InventoryItem> AddInventoryItemAsync(InventoryItem item);
        Task<InventoryItem> UpdateInventoryItemAsync(InventoryItem item);
        Task<bool> DeleteInventoryItemAsync(int id);
    }
}

