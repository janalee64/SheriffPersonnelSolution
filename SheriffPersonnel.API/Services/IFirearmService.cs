using System.Collections.Generic;
using System.Threading.Tasks;
using SheriffPersonnel.Shared.DTOs;

namespace SheriffPersonnel.API.Services
{
    public interface IFirearmService
    {
        Task<IEnumerable<FirearmsDto>> GetAllFirearmsAsync();
        Task<IEnumerable<FirearmsDto>> GetFirearmsByEmployeeIdAsync(int employeeId);
        Task<FirearmsDto?> GetFirearmByIdAsync(int id);
        Task<FirearmsDto> CreateFirearmAsync(FirearmsDto dto);
        Task<FirearmsDto?> UpdateFirearmAsync(int id, FirearmsDto dto);
        Task<bool> DeleteFirearmAsync(int id);
        Task<IEnumerable<MakesDto>> GetMakesAsync();
    }
}

