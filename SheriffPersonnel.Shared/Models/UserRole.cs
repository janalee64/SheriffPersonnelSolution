using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SheriffPersonnel.Shared.Models
{
    public class UserRole
    {
        public int UserID { get; set; }

        [ForeignKey(nameof(UserID))]
        public virtual User? User { get; set; }

        public int RoleID { get; set; }

        [ForeignKey(nameof(RoleID))]
        public virtual Role? Role { get; set; }

        public DateTime AssignedDate { get; set; } = DateTime.Now;

        [StringLength(100)]
        public string? AssignedBy { get; set; }
    }
}

