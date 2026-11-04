using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SheriffPersonnel.Shared.Data;
using SheriffPersonnel.Shared.Models;
using ProgramModel = SheriffPersonnel.Shared.Models.Program;

namespace SheriffPersonnel.API.Repositories
{
    public class EmployeeChangeRepository : IEmployeeChangeRepository
    {
        private readonly DBContext _dbContext;

        public EmployeeChangeRepository(DBContext dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public async Task<IEnumerable<EmployeeChange>> GetChangesByEmployeeIdAsync(int employeeId)
        {
            return await _dbContext.EmployeeChanges
                .Include(c => c.Program)
                .Include(c => c.Rank)
                .Include(c => c.Unit)
                .Include(c => c.LastUser)
                .Where(c => c.EmployeeID == employeeId)
                .OrderByDescending(c => c.HistoryDate)
                .ToListAsync();
        }

        public async Task<EmployeeChange?> GetChangeByIdAsync(int id)
        {
            return await _dbContext.EmployeeChanges
                .Include(c => c.Program)
                .Include(c => c.Rank)
                .Include(c => c.Unit)
                .Include(c => c.LastUser)
                .FirstOrDefaultAsync(c => c.HistoryID == id);
        }

        public async Task<EmployeeChange> AddChangeAsync(EmployeeChange change)
        {
            if (change == null) throw new ArgumentNullException(nameof(change));

            change.LastUpdated = DateTime.Now;
            if (change.HistoryDate == default)
            {
                change.HistoryDate = DateTime.Now;
            }

            await _dbContext.EmployeeChanges.AddAsync(change);
            await _dbContext.SaveChangesAsync();
            return change;
        }

        public async Task<EmployeeChange> UpdateChangeAsync(EmployeeChange change)
        {
            if (change == null) throw new ArgumentNullException(nameof(change));

            change.LastUpdated = DateTime.Now;

            var entry = _dbContext.Entry(change);
            if (entry.State == EntityState.Detached)
            {
                _dbContext.EmployeeChanges.Update(change);
            }

            await _dbContext.SaveChangesAsync();
            return change;
        }

        public async Task<bool> DeleteChangeAsync(int id)
        {
            var change = await _dbContext.EmployeeChanges.FindAsync(id);
            if (change == null) return false;

            _dbContext.EmployeeChanges.Remove(change);
            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<ProgramModel>> GetProgramsAsync()
        {
            return await _dbContext.Programs.OrderBy(p => p.ProgramName).ToListAsync();
        }

        public async Task<IEnumerable<Rank>> GetRanksAsync()
        {
            return await _dbContext.Ranks.OrderBy(r => r.RankName).ToListAsync();
        }

        public async Task<IEnumerable<Unit>> GetUnitsAsync()
        {
            return await _dbContext.Units.OrderBy(u => u.UnitName).ToListAsync();
        }
    }
}

