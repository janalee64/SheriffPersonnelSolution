using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SheriffPersonnel.Shared.Models
{
    public class Program
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ProgramID { get; set; }

        [Required]
        [StringLength(100)]
        public string ProgramName { get; set; } = string.Empty;

        [StringLength(255)]
        public string? Description { get; set; }

        public bool? IsActive { get; set; } = true;

        public virtual ICollection<EmployeeChange> EmployeeChanges { get; set; } = new List<EmployeeChange>();
    }
}

