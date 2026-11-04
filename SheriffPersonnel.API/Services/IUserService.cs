using System.Collections.Generic;
using System.Threading.Tasks;
using SheriffPersonnel.Shared.DTOs;

namespace SheriffPersonnel.API.Services
{
    public interface IUserService
    {
        Task<IEnumerable<UserDTO>> GetAllUsersAsync();
        Task<UserDTO?> GetUserByIdAsync(int id);
        Task<UserDTO?> GetUserByEntraOrUpnAsync(string? entraObjectId, string? upn);
        Task<UserDTO> CreateUserAsync(UserCreateUpdateDto dto, string? assignedBy = null);
        Task<UserDTO?> UpdateUserAsync(int id, UserCreateUpdateDto dto, string? assignedBy = null);
        Task<bool> DeleteUserAsync(int id);
        Task<IEnumerable<RoleDTO>> GetRolesAsync();
    }
}

