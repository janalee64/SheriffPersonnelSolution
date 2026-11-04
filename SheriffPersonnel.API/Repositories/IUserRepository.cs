using System.Collections.Generic;
using System.Threading.Tasks;
using SheriffPersonnel.Shared.Models;

namespace SheriffPersonnel.API.Repositories
{
    public interface IUserRepository
    {
        Task<IEnumerable<User>> GetAllUsersAsync();
        Task<User?> GetUserByIdAsync(int id);
        Task<User?> GetUserByEntraObjectIdAsync(string entraObjectId);
        Task<User?> GetUserByEntraOrUpnAsync(string? entraObjectId, string? upn);
        Task<User> CreateUserAsync(User user, IEnumerable<int> roleIds, string? assignedBy = null);
        Task<User?> UpdateUserAsync(int id, User updatedUser, IEnumerable<int> roleIds, string? assignedBy = null);
        Task<bool> DeleteUserAsync(int id);
        Task<IEnumerable<Role>> GetRolesAsync();
    }
}

