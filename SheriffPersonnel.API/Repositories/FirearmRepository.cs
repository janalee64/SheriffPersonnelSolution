using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SheriffPersonnel.Shared.Data;
using SheriffPersonnel.Shared.Models;

namespace SheriffPersonnel.API.Repositories
{
    public class FirearmRepository : IFirearmRepository
    {
        private readonly DBContext _dbContext;

        public FirearmRepository(DBContext dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public async Task<IEnumerable<Firearm>> GetAllFirearmsAsync()
        {
            return await _dbContext.Firearms
                .Include(f => f.Make)
                .Include(f => f.Status)
                .Include(f => f.LastUser)
                .Include(f => f.Employee)
                    .ThenInclude(e => e.Status)
                .OrderBy(f => f.SerialNumber)
                .ToListAsync();
        }

        public async Task<IEnumerable<Firearm>> GetFirearmsByEmployeeIdAsync(int employeeId)
        {
            return await _dbContext.Firearms
                .Include(f => f.Make)
                .Include(f => f.Status)
                .Include(f => f.LastUser)
                .Include(f => f.Employee)
                    .ThenInclude(e => e.Status)
                .Where(f => f.EmployeeID == employeeId)
                .OrderBy(f => f.Status != null ? (f.Status.StatusDescription ?? f.Status.StatusCode) : "")
                .ThenBy(f => f.Make != null ? f.Make.MakeName : "")
                .ThenBy(f => f.ModelNumber)
                .ToListAsync();
        }

        public async Task<Firearm?> GetFirearmByIdAsync(int id)
        {
            return await _dbContext.Firearms
                .Include(f => f.Make)
                .Include(f => f.Status)
                .Include(f => f.LastUser)
                .Include(f => f.Employee)
                    .ThenInclude(e => e.Status)
                .FirstOrDefaultAsync(f => f.FirearmID == id);
        }

        public async Task<Firearm> AddFirearmAsync(Firearm firearm)
        {
            if (firearm == null) throw new ArgumentNullException(nameof(firearm));

            firearm.LastUpdated = DateTime.Now;
            await _dbContext.Firearms.AddAsync(firearm);
            await _dbContext.SaveChangesAsync();
            return firearm;
        }

        public async Task<Firearm> UpdateFirearmAsync(Firearm firearm)
        {
            if (firearm == null) throw new ArgumentNullException(nameof(firearm));

            firearm.LastUpdated = DateTime.Now;

            var entry = _dbContext.Entry(firearm);
            if (entry.State == EntityState.Detached)
            {
                _dbContext.Firearms.Update(firearm);
            }

            await _dbContext.SaveChangesAsync();
            return firearm;
        }

        public async Task<bool> DeleteFirearmAsync(int id)
        {
            var firearm = await _dbContext.Firearms.FindAsync(id);
            if (firearm == null) return false;

            _dbContext.Firearms.Remove(firearm);
            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<Make>> GetMakesAsync()
        {
            return await _dbContext.Makes
                .OrderBy(m => m.MakeName)
                .ToListAsync();
        }
    }
}

