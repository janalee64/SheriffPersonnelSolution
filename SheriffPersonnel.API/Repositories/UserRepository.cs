using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SheriffPersonnel.Shared.Data;
using SheriffPersonnel.Shared.Models;

namespace SheriffPersonnel.API.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly DBContext _dbContext;

        public UserRepository(DBContext dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            return await _dbContext.Users
                .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                .OrderBy(u => u.DisplayName)
                .ToListAsync();
        }

        public async Task<User?> GetUserByIdAsync(int id)
        {
            return await _dbContext.Users
                .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(u => u.UserID == id);
        }

        public async Task<User?> GetUserByEntraObjectIdAsync(string entraObjectId)
        {
            return await _dbContext.Users
                .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(u => u.EntraObjectID == entraObjectId);
        }

        public async Task<User?> GetUserByEntraOrUpnAsync(string? entraObjectId, string? upn)
        {
            if (string.IsNullOrEmpty(entraObjectId) && string.IsNullOrEmpty(upn))
                return null;

            var query = _dbContext.Users
                .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                .AsQueryable();

            if (!string.IsNullOrEmpty(entraObjectId) && !string.IsNullOrEmpty(upn))
            {
                return await query.FirstOrDefaultAsync(u => u.EntraObjectID == entraObjectId || u.UserPrincipalName == upn);
            }
            if (!string.IsNullOrEmpty(entraObjectId))
            {
                return await query.FirstOrDefaultAsync(u => u.EntraObjectID == entraObjectId);
            }
            return await query.FirstOrDefaultAsync(u => u.UserPrincipalName == upn);
        }

        public async Task<User> CreateUserAsync(User user, IEnumerable<int> roleIds, string? assignedBy = null)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));

            user.CreatedDate = DateTime.Now;
            user.LastUpdated = DateTime.Now;

            await _dbContext.Users.AddAsync(user);
            await _dbContext.SaveChangesAsync();

            var assignedByValue = string.IsNullOrWhiteSpace(assignedBy) ? "System Administrator" : assignedBy;

            if (roleIds != null && roleIds.Any())
            {
                foreach (var roleId in roleIds.Distinct())
                {
                    var userRole = new UserRole
                    {
                        UserID = user.UserID,
                        RoleID = roleId,
                        AssignedDate = DateTime.Now,
                        AssignedBy = assignedByValue
                    };
                    await _dbContext.UserRoles.AddAsync(userRole);
                }
                await _dbContext.SaveChangesAsync();
            }

            return await GetUserByIdAsync(user.UserID) ?? user;
        }

        public async Task<User?> UpdateUserAsync(int id, User updatedUser, IEnumerable<int> roleIds, string? assignedBy = null)
        {
            if (updatedUser == null) throw new ArgumentNullException(nameof(updatedUser));

            var existingUser = await _dbContext.Users
                .Include(u => u.UserRoles)
                .FirstOrDefaultAsync(u => u.UserID == id);

            if (existingUser == null) return null;

            existingUser.EntraObjectID = updatedUser.EntraObjectID;
            existingUser.UserPrincipalName = updatedUser.UserPrincipalName;
            existingUser.DisplayName = updatedUser.DisplayName;
            existingUser.EmployeeID = updatedUser.EmployeeID;
            existingUser.IsActive = updatedUser.IsActive;
            existingUser.LastUpdated = DateTime.Now;

            // Sync UserRoles
            var desiredRoleIds = roleIds?.Distinct().ToList() ?? new List<int>();
            var currentRoleIds = existingUser.UserRoles.Select(ur => ur.RoleID).ToList();

            // Remove roles no longer assigned
            var rolesToRemove = existingUser.UserRoles
                .Where(ur => !desiredRoleIds.Contains(ur.RoleID))
                .ToList();

            foreach (var ur in rolesToRemove)
            {
                _dbContext.UserRoles.Remove(ur);
            }

            // Add new roles
            var rolesToAdd = desiredRoleIds
                .Where(rId => !currentRoleIds.Contains(rId))
                .ToList();

            var assignedByValue = string.IsNullOrWhiteSpace(assignedBy) ? "System Administrator" : assignedBy;

            foreach (var rId in rolesToAdd)
            {
                existingUser.UserRoles.Add(new UserRole
                {
                    UserID = existingUser.UserID,
                    RoleID = rId,
                    AssignedDate = DateTime.Now,
                    AssignedBy = assignedByValue
                });
            }

            await _dbContext.SaveChangesAsync();
            return await GetUserByIdAsync(id);
        }

        public async Task<bool> DeleteUserAsync(int id)
        {
            var user = await _dbContext.Users.FindAsync(id);
            if (user == null) return false;

            _dbContext.Users.Remove(user);
            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<Role>> GetRolesAsync()
        {
            return await _dbContext.Roles
                .OrderBy(r => r.RoleName)
                .ToListAsync();
        }
    }
}

