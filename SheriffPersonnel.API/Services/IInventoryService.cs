using System.Collections.Generic;
using System.Threading.Tasks;
using SheriffPersonnel.Shared.DTOs;

namespace SheriffPersonnel.API.Services
{
    public interface IInventoryService
    {
        Task<IEnumerable<EmployeeInventoryAssignmentsDto>> GetAllAssignmentsAsync(
            string? productName = null,
            int? programId = null,
            int? statusId = null,
            DateTime? startDate = null,
            DateTime? endDate = null,
            int? maxResults = null);
        Task<IEnumerable<EmployeeInventoryAssignmentsDto>> GetAssignmentsByEmployeeIdAsync(int employeeId);
        Task<EmployeeInventoryAssignmentsDto?> GetAssignmentByIdAsync(int id);
        Task<EmployeeInventoryAssignmentsDto> CreateAssignmentAsync(EmployeeInventoryAssignmentsDto dto);
        Task<MultipleEquipmentAssignmentResultDto> AssignMultipleEquipmentAsync(MultipleEquipmentAssignmentRequestDto request);
        Task<EmployeeInventoryAssignmentsDto?> UpdateAssignmentAsync(int id, EmployeeInventoryAssignmentsDto dto);
        Task<bool> DeleteAssignmentAsync(int id);

        Task<IEnumerable<InventoryItemsDto>> GetInventoryItemsAsync(bool? onlyActive = null);
        Task<InventoryItemsDto?> GetInventoryItemByIdAsync(int id);
        Task<InventoryItemsDto> CreateInventoryItemAsync(InventoryItemsDto dto);
        Task<InventoryItemsDto?> UpdateInventoryItemAsync(int id, InventoryItemsDto dto);
        Task<bool> DeleteInventoryItemAsync(int id);
    }
}

