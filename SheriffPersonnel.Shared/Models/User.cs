using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SheriffPersonnel.Shared.Models
{
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int UserID { get; set; }

        [StringLength(40)]
        public string? EntraObjectID { get; set; }

        [Required(ErrorMessage = "User principal name is required.")]
        [StringLength(256)]
        public string UserPrincipalName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Display name is required.")]
        [StringLength(150)]
        public string DisplayName { get; set; } = string.Empty;

        public int? EmployeeID { get; set; }

        public bool? IsActive { get; set; } = true;

        public DateTime? LastLoginDate { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        public DateTime LastUpdated { get; set; } = DateTime.Now;

        public virtual ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
    }
}

