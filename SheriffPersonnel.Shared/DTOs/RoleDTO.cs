using System;

namespace SheriffPersonnel.Shared.DTOs
{
    public class RoleDTO
    {
        public int RoleID { get; set; }
        public string RoleName { get; set; } = string.Empty;
        public string? RoleDescription { get; set; } = string.Empty;
    }
}

