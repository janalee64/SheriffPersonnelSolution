using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SheriffPersonnel.API.Repositories;
using SheriffPersonnel.Shared.DTOs;
using SheriffPersonnel.Shared.Models;

namespace SheriffPersonnel.API.Services
{
    public class InventoryService : IInventoryService
    {
        private readonly IInventoryRepository _repository;
        private readonly ILogger<InventoryService> _logger;

        public InventoryService(
            IInventoryRepository repository,
            ILogger<InventoryService> logger)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<IEnumerable<EmployeeInventoryAssignmentsDto>> GetAllAssignmentsAsync(
            string? productName = null,
            int? programId = null,
            int? statusId = null,
            DateTime? startDate = null,
            DateTime? endDate = null,
            int? maxResults = null)
        {
            try
            {
                bool hasFilters = !string.IsNullOrWhiteSpace(productName)
                    || (programId.HasValue && programId.Value > 0)
                    || (statusId.HasValue && statusId.Value > 0)
                    || startDate.HasValue
                    || endDate.HasValue;

                if (!hasFilters && !maxResults.HasValue)
                {
                    maxResults = 200;
                }

                var assignments = (await _repository.GetAllAssignmentsAsync(productName, programId, statusId, startDate, endDate, maxResults)).ToList();
                if (!assignments.Any())
                {
                    return Enumerable.Empty<EmployeeInventoryAssignmentsDto>();
                }

                var empIds = assignments.Select(a => a.EmployeeID).Distinct().ToList();
                var programDict = await _repository.GetLatestEmployeeProgramsAsync(empIds);

                return assignments.Select(a => MapToDto(a, programDict.TryGetValue(a.EmployeeID, out var p) ? p : string.Empty));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all inventory assignments.");
                throw;
            }
        }

        public async Task<IEnumerable<EmployeeInventoryAssignmentsDto>> GetAssignmentsByEmployeeIdAsync(int employeeId)
        {
            try
            {
                var assignments = await _repository.GetAssignmentsByEmployeeIdAsync(employeeId);
                return assignments.Select(a => MapToDto(a));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting inventory assignments for employee ID {EmployeeID}.", employeeId);
                throw;
            }
        }

        public async Task<EmployeeInventoryAssignmentsDto?> GetAssignmentByIdAsync(int id)
        {
            try
            {
                var assignment = await _repository.GetAssignmentByIdAsync(id);
                if (assignment == null) return null;
                return MapToDto(assignment);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting inventory assignment ID {EmpInventoryID}.", id);
                throw;
            }
        }

        public async Task<EmployeeInventoryAssignmentsDto> CreateAssignmentAsync(EmployeeInventoryAssignmentsDto dto)
        {
            try
            {
                if (dto == null) throw new ArgumentNullException(nameof(dto));

                var entity = MapToEntity(dto);
                var created = await _repository.AddAssignmentAsync(entity);

                var reloaded = await _repository.GetAssignmentByIdAsync(created.EmpInventoryID) ?? created;
                return MapToDto(reloaded);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating employee inventory assignment.");
                throw;
            }
        }

        public async Task<MultipleEquipmentAssignmentResultDto> AssignMultipleEquipmentAsync(MultipleEquipmentAssignmentRequestDto request)
        {
            try
            {
                if (request == null) throw new ArgumentNullException(nameof(request));
                if (request.EmployeeIDs == null || !request.EmployeeIDs.Any())
                    throw new ArgumentException("At least one employee must be specified.");
                if (request.InventoryIDs == null || !request.InventoryIDs.Any())
                    throw new ArgumentException("At least one equipment / inventory item must be specified.");

                var assignments = new List<EmployeeInventoryAssignment>();
                var issueDate = request.IssueDate != default ? request.IssueDate : DateTime.Now;
                var qty = request.Quantity > 0 ? request.Quantity : 1;

                foreach (var empId in request.EmployeeIDs.Distinct())
                {
                    foreach (var invId in request.InventoryIDs.Distinct())
                    {
                        assignments.Add(new EmployeeInventoryAssignment
                        {
                            EmployeeID = empId,
                            InventoryID = invId,
                            IssueDate = issueDate,
                            Quantity = qty,
                            SerialNumber = request.SerialNumber,
                            ExpireDate = request.ExpireDate,
                            ReturnDate = request.ReturnDate,
                            Notes = request.Notes,
                            LastUserID = request.LastUserID
                        });
                    }
                }

                var count = await _repository.AddAssignmentsAsync(assignments);

                return new MultipleEquipmentAssignmentResultDto
                {
                    AssignedEmployeeIDs = request.EmployeeIDs.Distinct().ToList(),
                    AssignedInventoryIDs = request.InventoryIDs.Distinct().ToList(),
                    TotalAssignmentsCreated = count
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating multiple equipment assignments.");
                throw;
            }
        }

        public async Task<EmployeeInventoryAssignmentsDto?> UpdateAssignmentAsync(int id, EmployeeInventoryAssignmentsDto dto)
        {
            try
            {
                if (dto == null) throw new ArgumentNullException(nameof(dto));

                var existing = await _repository.GetAssignmentByIdAsync(id);
                if (existing == null)
                {
                    _logger.LogWarning("Inventory assignment ID {EmpInventoryID} not found for update.", id);
                    return null;
                }

                existing.InventoryID = dto.InventoryID;
                existing.IssueDate = dto.IssueDate;
                existing.Quantity = dto.Quantity;
                existing.SerialNumber = dto.SerialNumber;
                existing.Notes = dto.Notes;
                existing.ReturnDate = dto.ReturnDate;
                existing.ExpireDate = dto.ExpireDate;
                existing.LastUserID = dto.LastUserID;

                var updated = await _repository.UpdateAssignmentAsync(existing);
                var reloaded = await _repository.GetAssignmentByIdAsync(updated.EmpInventoryID) ?? updated;
                var programDict = await _repository.GetLatestEmployeeProgramsAsync(new[] { reloaded.EmployeeID });
                return MapToDto(reloaded, programDict.TryGetValue(reloaded.EmployeeID, out var p) ? p : string.Empty);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating inventory assignment ID {EmpInventoryID}.", id);
                throw;
            }
        }

        public async Task<bool> DeleteAssignmentAsync(int id)
        {
            try
            {
                return await _repository.DeleteAssignmentAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting inventory assignment ID {EmpInventoryID}.", id);
                throw;
            }
        }

        public async Task<IEnumerable<InventoryItemsDto>> GetInventoryItemsAsync(bool? onlyActive = null)
        {
            try
            {
                var items = await _repository.GetInventoryItemsAsync(onlyActive);
                return items.Select(MapToItemDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting inventory items list.");
                throw;
            }
        }

        public async Task<InventoryItemsDto?> GetInventoryItemByIdAsync(int id)
        {
            try
            {
                var item = await _repository.GetInventoryItemByIdAsync(id);
                return item != null ? MapToItemDto(item) : null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting inventory item ID {InventoryID}.", id);
                throw;
            }
        }

        public async Task<InventoryItemsDto> CreateInventoryItemAsync(InventoryItemsDto dto)
        {
            try
            {
                if (dto == null) throw new ArgumentNullException(nameof(dto));

                var entity = MapToItemEntity(dto);
                var created = await _repository.AddInventoryItemAsync(entity);

                var reloaded = await _repository.GetInventoryItemByIdAsync(created.InventoryID) ?? created;
                return MapToItemDto(reloaded);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating inventory item.");
                throw;
            }
        }

        public async Task<InventoryItemsDto?> UpdateInventoryItemAsync(int id, InventoryItemsDto dto)
        {
            try
            {
                if (dto == null) throw new ArgumentNullException(nameof(dto));

                var existing = await _repository.GetInventoryItemByIdAsync(id);
                if (existing == null)
                {
                    _logger.LogWarning("Inventory item ID {InventoryID} not found for update.", id);
                    return null;
                }

                existing.SKU = dto.SKU;
                existing.ProductName = dto.ProductName;
                existing.CategoryID = dto.CategoryID;
                existing.SupplierID = dto.SupplierID;
                existing.LocationID = dto.LocationID;
                existing.BrandName = dto.BrandName;
                existing.Size = dto.Size;
                existing.Color = dto.Color;
                existing.UnitPrice = dto.UnitPrice;
                existing.ReorderPoint = dto.ReorderPoint;
                existing.StockQuantity = dto.StockQuantity;
                existing.LastPurchaseDate = dto.LastPurchaseDate;
                existing.LastUserID = dto.LastUserID;
                existing.IsActive = dto.IsActive;

                var updated = await _repository.UpdateInventoryItemAsync(existing);
                var reloaded = await _repository.GetInventoryItemByIdAsync(updated.InventoryID) ?? updated;
                return MapToItemDto(reloaded);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating inventory item ID {InventoryID}.", id);
                throw;
            }
        }

        public async Task<bool> DeleteInventoryItemAsync(int id)
        {
            try
            {
                return await _repository.DeleteInventoryItemAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting inventory item ID {InventoryID}.", id);
                throw;
            }
        }

        private static InventoryItemsDto MapToItemDto(InventoryItem i)
        {
            return new InventoryItemsDto
            {
                InventoryID = i.InventoryID,
                SKU = i.SKU,
                ProductName = i.ProductName,
                CategoryID = i.CategoryID,
                CategoryName = i.Category?.CategoryName,
                SupplierID = i.SupplierID,
                SupplierName = i.Supplier?.SupplierName,
                LocationID = i.LocationID,
                LocationName = i.Location?.LocationName,
                BrandName = i.BrandName,
                Size = i.Size,
                Color = i.Color,
                UnitPrice = i.UnitPrice,
                ReorderPoint = i.ReorderPoint,
                StockQuantity = i.StockQuantity,
                LastPurchaseDate = i.LastPurchaseDate,
                LastUserID = i.LastUserID,
                IsActive = i.IsActive ?? true,
                LastUpdated = i.LastUpdated
            };
        }

        private static InventoryItem MapToItemEntity(InventoryItemsDto dto)
        {
            return new InventoryItem
            {
                InventoryID = dto.InventoryID,
                SKU = dto.SKU,
                ProductName = dto.ProductName,
                CategoryID = dto.CategoryID,
                SupplierID = dto.SupplierID,
                LocationID = dto.LocationID,
                BrandName = dto.BrandName,
                Size = dto.Size,
                Color = dto.Color,
                UnitPrice = dto.UnitPrice,
                ReorderPoint = dto.ReorderPoint,
                StockQuantity = dto.StockQuantity,
                LastPurchaseDate = dto.LastPurchaseDate,
                LastUserID = dto.LastUserID,
                IsActive = dto.IsActive
            };
        }

        private static EmployeeInventoryAssignmentsDto MapToDto(EmployeeInventoryAssignment entity, string programName = "")
        {
            var empName = entity.Employee != null
                ? $"{entity.Employee.LastName}, {entity.Employee.FirstName}".Trim(',', ' ')
                : string.Empty;

            var empStatus = entity.Employee?.Status?.StatusDescription
                ?? entity.Employee?.Status?.StatusCode
                ?? string.Empty;

            if (string.IsNullOrEmpty(programName) && entity.Employee?.EmployeeChanges != null && entity.Employee.EmployeeChanges.Count > 0)
            {
                programName = entity.Employee.EmployeeChanges
                    .Where(ec => ec.Program != null && !string.IsNullOrWhiteSpace(ec.Program.ProgramName))
                    .OrderByDescending(ec => ec.HistoryDate)
                    .ThenByDescending(ec => ec.HistoryID)
                    .Select(ec => ec.Program!.ProgramName)
                    .FirstOrDefault() ?? string.Empty;
            }

            return new EmployeeInventoryAssignmentsDto
            {
                EmpInventoryID = entity.EmpInventoryID,
                EmployeeID = entity.EmployeeID,
                EmployeeName = empName,
                ProgramName = programName,
                EmployeeStatus = empStatus,
                InventoryID = entity.InventoryID,
                ProductName = entity.InventoryItem?.ProductName,
                CategoryName = entity.InventoryItem?.Category?.CategoryName,
                IssueDate = entity.IssueDate,
                Quantity = entity.Quantity,
                SerialNumber = entity.SerialNumber,
                Notes = entity.Notes,
                ReturnDate = entity.ReturnDate,
                ExpireDate = entity.ExpireDate,
                LastUserID = entity.LastUserID,
                LastUserName = entity.LastUser?.DisplayName ?? entity.LastUser?.UserPrincipalName,
                LastUpdated = entity.LastUpdated
            };
        }

        private static EmployeeInventoryAssignment MapToEntity(EmployeeInventoryAssignmentsDto dto)
        {
            return new EmployeeInventoryAssignment
            {
                EmpInventoryID = dto.EmpInventoryID,
                EmployeeID = dto.EmployeeID,
                InventoryID = dto.InventoryID,
                IssueDate = dto.IssueDate,
                Quantity = dto.Quantity,
                SerialNumber = dto.SerialNumber,
                Notes = dto.Notes,
                ReturnDate = dto.ReturnDate,
                ExpireDate = dto.ExpireDate,
                LastUserID = dto.LastUserID
            };
        }
    }
}

