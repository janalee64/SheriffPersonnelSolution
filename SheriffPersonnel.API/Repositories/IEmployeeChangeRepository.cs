using System.Collections.Generic;
using System.Threading.Tasks;
using SheriffPersonnel.Shared.Models;
using ProgramModel = SheriffPersonnel.Shared.Models.Program;

namespace SheriffPersonnel.API.Repositories
{
    public interface IEmployeeChangeRepository
    {
        Task<IEnumerable<EmployeeChange>> GetChangesByEmployeeIdAsync(int employeeId);
        Task<EmployeeChange?> GetChangeByIdAsync(int id);
        Task<EmployeeChange> AddChangeAsync(EmployeeChange change);
        Task<EmployeeChange> UpdateChangeAsync(EmployeeChange change);
        Task<bool> DeleteChangeAsync(int id);

        Task<IEnumerable<ProgramModel>> GetProgramsAsync();
        Task<IEnumerable<Rank>> GetRanksAsync();
        Task<IEnumerable<Unit>> GetUnitsAsync();
    }
}

