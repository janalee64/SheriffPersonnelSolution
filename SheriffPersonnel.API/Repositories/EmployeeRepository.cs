using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SheriffPersonnel.Shared.Data;
using SheriffPersonnel.Shared.DTOs;
using SheriffPersonnel.Shared.Models;

namespace SheriffPersonnel.API.Repositories
{
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly DBContext _dbContext;

        public EmployeeRepository(DBContext dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public async Task<IEnumerable<Employee>> SearchEmployeesAsync(EmployeeSearchRequestDto request)
        {
            IQueryable<Employee> query = _dbContext.Employees
                .AsSplitQuery()
                .Include(e => e.Status)
                .Include(e => e.EmployeeChanges)
                    .ThenInclude(ec => ec.Program)
                .Include(e => e.EmployeeChanges)
                    .ThenInclude(ec => ec.Unit);

            if (request != null)
            {
                if (!string.IsNullOrWhiteSpace(request.FirstName))
                {
                    var term = request.FirstName.Trim();
                    query = query.Where(e => e.FirstName != null && EF.Functions.Like(e.FirstName, $"%{term}%"));
                }

                if (!string.IsNullOrWhiteSpace(request.LastName))
                {
                    var term = request.LastName.Trim();
                    query = query.Where(e => e.LastName != null && EF.Functions.Like(e.LastName, $"%{term}%"));
                }

                if (!string.IsNullOrWhiteSpace(request.BadgeNumber))
                {
                    var term = request.BadgeNumber.Trim();
                    if (int.TryParse(term, out int badgeNum))
                    {
                        query = query.Where(e => e.BadgeNumber == badgeNum);
                    }
                    else
                    {
                        query = query.Where(e => e.BadgeNumber != null && EF.Functions.Like(e.BadgeNumber.ToString(), $"%{term}%"));
                    }
                }

                if (!string.IsNullOrWhiteSpace(request.Status))
                {
                    var term = request.Status.Trim();
                    bool isNumeric = int.TryParse(term, out int statusId);

                    query = query.Where(e => e.Status != null &&
                        ((isNumeric && e.StatusID == statusId) ||
                         (e.Status.StatusDescription != null && (e.Status.StatusDescription == term || e.Status.StatusDescription.ToLower() == term.ToLower())) ||
                         (e.Status.StatusCode != null && (e.Status.StatusCode == term || e.Status.StatusCode.ToLower() == term.ToLower()))));
                }

                if (request.ProgramID.HasValue && request.ProgramID.Value > 0)
                {
                    query = query.Where(e => e.EmployeeChanges.Any(ec => ec.ProgramID == request.ProgramID.Value));
                }

                if (request.UnitID.HasValue && request.UnitID.Value > 0)
                {
                    query = query.Where(e => e.EmployeeChanges.Any(ec => ec.UnitID == request.UnitID.Value));
                }

                if (request.EmployeeFlag.HasValue)
                {
                    query = query.Where(e => e.EmployeeFlag == request.EmployeeFlag.Value);
                }

                if (!string.IsNullOrWhiteSpace(request.SortBy))
                {
                    query = request.SortBy.Trim().ToLowerInvariant() switch
                    {
                        "name" or "lastname" => request.SortDescending
                            ? query.OrderByDescending(e => e.LastName).ThenByDescending(e => e.FirstName)
                            : query.OrderBy(e => e.LastName).ThenBy(e => e.FirstName),
                        "hire date" or "hiredate" => request.SortDescending
                            ? query.OrderByDescending(e => e.HireDate)
                            : query.OrderBy(e => e.HireDate),
                        "inactive date" or "inactivedate" => request.SortDescending
                            ? query.OrderByDescending(e => e.InactiveDate)
                            : query.OrderBy(e => e.InactiveDate),
                        "badgenumber" => request.SortDescending
                            ? query.OrderByDescending(e => e.BadgeNumber)
                            : query.OrderBy(e => e.BadgeNumber),
                        "firstname" => request.SortDescending
                            ? query.OrderByDescending(e => e.FirstName)
                            : query.OrderBy(e => e.FirstName),
                        "status" => request.SortDescending
                            ? query.OrderByDescending(e => e.Status != null ? e.Status.StatusDescription : "")
                            : query.OrderBy(e => e.Status != null ? e.Status.StatusDescription : ""),
                        _ => query.OrderBy(e => e.LastName).ThenBy(e => e.FirstName)
                    };
                }
                else
                {
                    query = query.OrderBy(e => e.LastName).ThenBy(e => e.FirstName);
                }
            }
            else
            {
                query = query.OrderBy(e => e.LastName).ThenBy(e => e.FirstName);
            }

            return await query.ToListAsync();
        }

        public async Task<IEnumerable<Employee>> GetActiveEmployeesAsync()
        {
            var today = DateOnly.FromDateTime(DateTime.Today);
            return await _dbContext.Employees
                .Include(e => e.Status)
                .Include(e => e.EmployeeChanges)
                    .ThenInclude(ec => ec.Program)
                .Include(e => e.EmployeeChanges)
                    .ThenInclude(ec => ec.Unit)
                .Where(e => (e.InactiveDate == null || e.InactiveDate > today) &&
                            (e.Status == null || (e.Status.StatusCode != "INACT" &&
                                                  e.Status.StatusCode != "TERM" &&
                                                  (e.Status.StatusDescription == null ||
                                                   (!e.Status.StatusDescription.ToLower().Contains("inactive") &&
                                                    !e.Status.StatusDescription.ToLower().Contains("terminated") &&
                                                    !e.Status.StatusDescription.ToLower().Contains("retired") &&
                                                    !e.Status.StatusDescription.ToLower().Contains("resigned"))))))
                .OrderBy(e => e.LastName)
                .ThenBy(e => e.FirstName)
                .ToListAsync();
        }

        public async Task<IEnumerable<Status>> GetStatusesAsync()
        {
            return await _dbContext.Statuses.OrderBy(s => s.StatusDescription).ToListAsync();
        }

        public async Task<Employee?> GetEmployeeByIdAsync(int id)
        {
            return await _dbContext.Employees
                .Include(e => e.Status)
                .FirstOrDefaultAsync(e => e.EmployeeID == id);
        }

        public async Task<Employee> AddEmployeeAsync(Employee employee)
        {
            if (employee == null) throw new ArgumentNullException(nameof(employee));

            employee.LastUpdated = DateTime.Now;
            await _dbContext.Employees.AddAsync(employee);
            await _dbContext.SaveChangesAsync();
            return employee;
        }

        public async Task<Employee> UpdateEmployeeAsync(Employee employee)
        {
            if (employee == null) throw new ArgumentNullException(nameof(employee));

            employee.LastUpdated = DateTime.Now;

            var entry = _dbContext.Entry(employee);
            if (entry.State == EntityState.Detached)
            {
                _dbContext.Employees.Update(employee);
            }

            await _dbContext.SaveChangesAsync();
            return employee;
        }

        public async Task<bool> DeleteEmployeeAsync(int id)
        {
            var employee = await _dbContext.Employees.FindAsync(id);
            if (employee == null) return false;

            _dbContext.Employees.Remove(employee);
            await _dbContext.SaveChangesAsync();
            return true;
        }
    }
}
