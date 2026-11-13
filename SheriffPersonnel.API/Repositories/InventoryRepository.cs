using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SheriffPersonnel.Shared.Data;
using SheriffPersonnel.Shared.Models;

namespace SheriffPersonnel.API.Repositories
{
    public class InventoryRepository : IInventoryRepository
    {
        private readonly DBContext _dbContext;

        public InventoryRepository(DBContext dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public async Task<IEnumerable<EmployeeInventoryAssignment>> GetAllAssignmentsAsync(
            string? productName = null,
            int? programId = null,
            int? statusId = null,
            DateTime? startDate = null,
            DateTime? endDate = null,
            int? maxResults = null)
        {
            var query = _dbContext.EmployeeInventoryAssignments
                .AsNoTracking()
                .Include(a => a.InventoryItem)
                    .ThenInclude(i => i!.Category)
                .Include(a => a.Employee)
                    .ThenInclude(e => e!.Status)
                .Include(a => a.LastUser)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(productName))
            {
                var term = productName.Trim();
                var matchingInventoryIds = await _dbContext.InventoryItems
                    .AsNoTracking()
                    .Where(i => i.ProductName != null && EF.Functions.Like(i.ProductName, $"%{term}%"))
                    .Select(i => i.InventoryID)
                    .ToListAsync();

                if (!matchingInventoryIds.Any())
                {
                    return Enumerable.Empty<EmployeeInventoryAssignment>();
                }

                query = query.Where(a => matchingInventoryIds.Contains(a.InventoryID));
            }

            if (statusId.HasValue && statusId.Value > 0)
            {
                query = query.Where(a => a.Employee != null && a.Employee.StatusID == statusId.Value);
            }

            if (startDate.HasValue)
            {
                var start = startDate.Value.Date;
                query = query.Where(a => a.IssueDate >= start);
            }

            if (endDate.HasValue)
            {
                var end = endDate.Value.Date.AddDays(1);
                query = query.Where(a => a.IssueDate < end);
            }

            if (programId.HasValue && programId.Value > 0)
            {
                var targetProgramId = programId.Value;
                query = query.Where(a => a.Employee != null && a.Employee.EmployeeChanges.Any(ec => ec.ProgramID == targetProgramId));
            }

            query = query.OrderByDescending(a => a.IssueDate);

            if (maxResults.HasValue && maxResults.Value > 0)
            {
                query = query.Take(maxResults.Value);
            }

            return await query.ToListAsync();
        }

        public async Task<IEnumerable<EmployeeInventoryAssignment>> GetAssignmentsByEmployeeIdAsync(int employeeId)
        {
            return await _dbContext.EmployeeInventoryAssignments
                .AsNoTracking()
                .Include(a => a.InventoryItem)
                    .ThenInclude(i => i!.Category)
                .Include(a => a.Employee)
                    .ThenInclude(e => e!.Status)
                .Include(a => a.LastUser)
                .Where(a => a.EmployeeID == employeeId)
                .OrderByDescending(a => a.IssueDate)
                .ThenByDescending(a => a.EmpInventoryID)
                .ToListAsync();
        }

        public async Task<EmployeeInventoryAssignment?> GetAssignmentByIdAsync(int id)
        {
            return await _dbContext.EmployeeInventoryAssignments
                .AsNoTracking()
                .Include(a => a.InventoryItem)
                    .ThenInclude(i => i!.Category)
                .Include(a => a.Employee)
                    .ThenInclude(e => e!.Status)
                .Include(a => a.LastUser)
                .FirstOrDefaultAsync(a => a.EmpInventoryID == id);
        }

        public async Task<Dictionary<int, string>> GetLatestEmployeeProgramsAsync(IEnumerable<int> employeeIds)
        {
            var ids = employeeIds.Distinct().ToList();
            if (!ids.Any()) return new Dictionary<int, string>();

            var changes = await _dbContext.EmployeeChanges
                .AsNoTracking()
                .Where(ec => ids.Contains(ec.EmployeeID) && ec.ProgramID != null && ec.Program != null && ec.Program.ProgramName != null)
                .Select(ec => new
                {
                    ec.EmployeeID,
                    ec.HistoryID,
                    ProgramName = ec.Program!.ProgramName
                })
                .ToListAsync();

            return changes
                .GroupBy(x => x.EmployeeID)
                .ToDictionary(
                    g => g.Key,
                    g => g.OrderByDescending(x => x.HistoryID).First().ProgramName
                );
        }

        public async Task<EmployeeInventoryAssignment> AddAssignmentAsync(EmployeeInventoryAssignment assignment)
        {
            if (assignment == null) throw new ArgumentNullException(nameof(assignment));

            assignment.LastUpdated = DateTime.Now;
            if (assignment.IssueDate == default)
            {
                assignment.IssueDate = DateTime.Now;
            }

            await _dbContext.EmployeeInventoryAssignments.AddAsync(assignment);
            await _dbContext.SaveChangesAsync();
            return assignment;
        }

        public async Task<int> AddAssignmentsAsync(IEnumerable<EmployeeInventoryAssignment> assignments)
        {
            if (assignments == null || !assignments.Any()) return 0;

            var now = DateTime.Now;
            foreach (var assignment in assignments)
            {
                assignment.LastUpdated = now;
                if (assignment.IssueDate == default)
                {
                    assignment.IssueDate = now;
                }
            }

            await _dbContext.EmployeeInventoryAssignments.AddRangeAsync(assignments);
            return await _dbContext.SaveChangesAsync();
        }

        public async Task<EmployeeInventoryAssignment> UpdateAssignmentAsync(EmployeeInventoryAssignment assignment)
        {
            if (assignment == null) throw new ArgumentNullException(nameof(assignment));

            assignment.LastUpdated = DateTime.Now;

            var existing = await _dbContext.EmployeeInventoryAssignments.FindAsync(assignment.EmpInventoryID);
            if (existing != null)
            {
                existing.InventoryID = assignment.InventoryID;
                existing.IssueDate = assignment.IssueDate;
                existing.Quantity = assignment.Quantity;
                existing.SerialNumber = assignment.SerialNumber;
                existing.Notes = assignment.Notes;
                existing.ReturnDate = assignment.ReturnDate;
                existing.ExpireDate = assignment.ExpireDate;
                existing.LastUserID = assignment.LastUserID;
                existing.LastUpdated = assignment.LastUpdated;
                await _dbContext.SaveChangesAsync();
                return existing;
            }

            var entry = _dbContext.Entry(assignment);
            if (entry.State == EntityState.Detached)
            {
                _dbContext.EmployeeInventoryAssignments.Update(assignment);
            }

            await _dbContext.SaveChangesAsync();
            return assignment;
        }

        public async Task<bool> DeleteAssignmentAsync(int id)
        {
            var assignment = await _dbContext.EmployeeInventoryAssignments.FindAsync(id);
            if (assignment == null) return false;

            _dbContext.EmployeeInventoryAssignments.Remove(assignment);
            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<InventoryItem>> GetInventoryItemsAsync(bool? onlyActive = null)
        {
            var query = _dbContext.InventoryItems
                .AsNoTracking()
                .Include(i => i.Category)
                .Include(i => i.Supplier)
                .Include(i => i.Location)
                .Include(i => i.LastUser)
                .AsQueryable();

            if (onlyActive.HasValue)
            {
                if (onlyActive.Value)
                {
                    query = query.Where(i => i.IsActive == true);
                }
                else
                {
                    query = query.Where(i => i.IsActive == false || i.IsActive == null);
                }
            }

            return await query
                .OrderBy(i => i.ProductName)
                .ToListAsync();
        }

        public async Task<InventoryItem?> GetInventoryItemByIdAsync(int id)
        {
            return await _dbContext.InventoryItems
                .Include(i => i.Category)
                .Include(i => i.Supplier)
                .Include(i => i.Location)
                .Include(i => i.LastUser)
                .FirstOrDefaultAsync(i => i.InventoryID == id);
        }

        public async Task<InventoryItem> AddInventoryItemAsync(InventoryItem item)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));

            item.LastUpdated = DateTime.Now;

            await _dbContext.InventoryItems.AddAsync(item);
            await _dbContext.SaveChangesAsync();
            return item;
        }

        public async Task<InventoryItem> UpdateInventoryItemAsync(InventoryItem item)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));

            item.LastUpdated = DateTime.Now;

            var existing = await _dbContext.InventoryItems.FindAsync(item.InventoryID);
            if (existing != null)
            {
                existing.SKU = item.SKU;
                existing.ProductName = item.ProductName;
                existing.CategoryID = item.CategoryID;
                existing.SupplierID = item.SupplierID;
                existing.LocationID = item.LocationID;
                existing.BrandName = item.BrandName;
                existing.Size = item.Size;
                existing.Color = item.Color;
                existing.UnitPrice = item.UnitPrice;
                existing.ReorderPoint = item.ReorderPoint;
                existing.StockQuantity = item.StockQuantity;
                existing.LastPurchaseDate = item.LastPurchaseDate;
                existing.LastUserID = item.LastUserID;
                existing.IsActive = item.IsActive;
                existing.LastUpdated = item.LastUpdated;
                await _dbContext.SaveChangesAsync();
                return existing;
            }

            var entry = _dbContext.Entry(item);
            if (entry.State == EntityState.Detached)
            {
                _dbContext.InventoryItems.Update(item);
            }

            await _dbContext.SaveChangesAsync();
            return item;
        }

        public async Task<bool> DeleteInventoryItemAsync(int id)
        {
            var item = await _dbContext.InventoryItems.FindAsync(id);
            if (item == null) return false;

            _dbContext.InventoryItems.Remove(item);
            await _dbContext.SaveChangesAsync();
            return true;
        }
    }
}

