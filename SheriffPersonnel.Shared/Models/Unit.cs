using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SheriffPersonnel.Shared.Models
{
    public class Unit
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int UnitID { get; set; }

        [Required]
        [StringLength(100)]
        public string UnitName { get; set; } = string.Empty;

        [StringLength(255)]
        public string? Notes { get; set; }

        public bool? IsActive { get; set; } = true;

        public virtual ICollection<EmployeeChange> EmployeeChanges { get; set; } = new List<EmployeeChange>();
    }
}

