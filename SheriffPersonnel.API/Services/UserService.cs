using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SheriffPersonnel.API.Repositories;
using SheriffPersonnel.Shared.DTOs;
using SheriffPersonnel.Shared.Models;

namespace SheriffPersonnel.API.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        }

        public async Task<IEnumerable<UserDTO>> GetAllUsersAsync()
        {
            var users = await _userRepository.GetAllUsersAsync();
            return users.Select(MapToDTO);
        }

        public async Task<UserDTO?> GetUserByIdAsync(int id)
        {
            var user = await _userRepository.GetUserByIdAsync(id);
            return user != null ? MapToDTO(user) : null;
        }

        public async Task<UserDTO?> GetUserByEntraOrUpnAsync(string? entraObjectId, string? upn)
        {
            var user = await _userRepository.GetUserByEntraOrUpnAsync(entraObjectId, upn);
            return user != null ? MapToDTO(user) : null;
        }

        public async Task<UserDTO> CreateUserAsync(UserCreateUpdateDto dto, string? assignedBy = null)
        {
            if (dto == null) throw new ArgumentNullException(nameof(dto));

            var userEntity = new User
            {
                EntraObjectID = dto.EntraObjectID,
                UserPrincipalName = dto.UserPrincipalName,
                DisplayName = dto.DisplayName,
                EmployeeID = dto.EmployeeID,
                IsActive = dto.IsActive
            };

            var createdUser = await _userRepository.CreateUserAsync(userEntity, dto.RoleIDs, assignedBy);
            return MapToDTO(createdUser);
        }

        public async Task<UserDTO?> UpdateUserAsync(int id, UserCreateUpdateDto dto, string? assignedBy = null)
        {
            if (dto == null) throw new ArgumentNullException(nameof(dto));

            var userEntity = new User
            {
                EntraObjectID = dto.EntraObjectID,
                UserPrincipalName = dto.UserPrincipalName,
                DisplayName = dto.DisplayName,
                EmployeeID = dto.EmployeeID,
                IsActive = dto.IsActive
            };

            var updatedUser = await _userRepository.UpdateUserAsync(id, userEntity, dto.RoleIDs, assignedBy);
            return updatedUser != null ? MapToDTO(updatedUser) : null;
        }

        public async Task<bool> DeleteUserAsync(int id)
        {
            return await _userRepository.DeleteUserAsync(id);
        }

        public async Task<IEnumerable<RoleDTO>> GetRolesAsync()
        {
            var roles = await _userRepository.GetRolesAsync();
            return roles.Select(r => new RoleDTO
            {
                RoleID = r.RoleID,
                RoleName = r.RoleName,
                RoleDescription = r.RoleDescription
            });
        }

        private static UserDTO MapToDTO(User user)
        {
            var roles = user.UserRoles?
                .Where(ur => ur.Role != null)
                .Select(ur => new RoleDTO
                {
                    RoleID = ur.Role!.RoleID,
                    RoleName = ur.Role.RoleName,
                    RoleDescription = ur.Role.RoleDescription
                })
                .ToList() ?? new List<RoleDTO>();

            var roleIds = roles.Select(r => r.RoleID).ToList();

            return new UserDTO
            {
                UserID = user.UserID,
                EntraObjectID = user.EntraObjectID,
                UserPrincipalName = user.UserPrincipalName,
                DisplayName = user.DisplayName,
                EmployeeID = user.EmployeeID,
                IsActive = user.IsActive ?? true,
                LastLoginDate = user.LastLoginDate,
                CreatedDate = user.CreatedDate,
                LastUpdated = user.LastUpdated,
                Roles = roles,
                RoleIDs = roleIds
            };
        }
    }
}

