using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SheriffPersonnel.Shared.Models
{
    public class Firearm
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int FirearmID { get; set; }

        [Required]
        [StringLength(100)]
        public string SerialNumber { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string ModelNumber { get; set; } = string.Empty;

        [Required]
        [StringLength(20)]
        public string Caliber { get; set; } = string.Empty;

        public bool Optic { get; set; }

        public int EmployeeID { get; set; }

        [ForeignKey(nameof(EmployeeID))]
        public virtual Employee Employee { get; set; } = null!;

        [StringLength(255)]
        public string? Notes { get; set; }

        public int? LastUserID { get; set; }

        [ForeignKey(nameof(LastUserID))]
        public virtual User? LastUser { get; set; }

        public DateTime LastUpdated { get; set; } = DateTime.Now;

        public int StatusID { get; set; }

        [ForeignKey(nameof(StatusID))]
        public virtual Status? Status { get; set; }

        public int? MakeID { get; set; }

        [ForeignKey(nameof(MakeID))]
        public virtual Make? Make { get; set; }

        public virtual ICollection<RangeStatistic> RangeStatistics { get; set; } = new List<RangeStatistic>();
    }
}

