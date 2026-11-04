using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SheriffPersonnel.Shared.Models
{
    public class Rank
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int RankID { get; set; }

        [Required]
        [StringLength(100)]
        public string RankName { get; set; } = string.Empty;

        public bool? IsActive { get; set; } = true;

        public virtual ICollection<EmployeeChange> EmployeeChanges { get; set; } = new List<EmployeeChange>();
    }
}

