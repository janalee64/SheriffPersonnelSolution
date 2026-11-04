using System.Collections.Generic;

namespace SheriffPersonnel.Shared.DTOs
{
    public class UserCreateUpdateDto
    {
        public string? EntraObjectID { get; set; }
        public string UserPrincipalName { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public int? EmployeeID { get; set; }
        public bool IsActive { get; set; } = true;
        public List<int> RoleIDs { get; set; } = new List<int>();
    }
}

