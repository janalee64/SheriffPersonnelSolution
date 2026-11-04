using System;
using System.Collections.Generic;

namespace SheriffPersonnel.Shared.DTOs
{
    public class UserDTO
    {
        public int UserID { get; set; }
        public string? EntraObjectID { get; set; }
        public string UserPrincipalName { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public int? EmployeeID { get; set; }
        public bool IsActive { get; set; }
        public DateTime? LastLoginDate { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime LastUpdated { get; set; }
        public List<RoleDTO> Roles { get; set; } = new List<RoleDTO>();
        public List<int> RoleIDs { get; set; } = new List<int>();
    }
}
