using System.Collections.Generic;
using System.Threading.Tasks;
using SheriffPersonnel.Shared.Models;

namespace SheriffPersonnel.API.Repositories
{
    public interface IFirearmRepository
    {
        Task<IEnumerable<Firearm>> GetAllFirearmsAsync();
        Task<IEnumerable<Firearm>> GetFirearmsByEmployeeIdAsync(int employeeId);
        Task<Firearm?> GetFirearmByIdAsync(int id);
        Task<Firearm> AddFirearmAsync(Firearm firearm);
        Task<Firearm> UpdateFirearmAsync(Firearm firearm);
        Task<bool> DeleteFirearmAsync(int id);
        Task<IEnumerable<Make>> GetMakesAsync();
    }
}

