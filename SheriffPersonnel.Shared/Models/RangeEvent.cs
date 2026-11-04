using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SheriffPersonnel.Shared.Models
{
    public class RangeEvent
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int RangeEventID { get; set; }

        [Required]
        public DateOnly RangeDate { get; set; }

        public int? RangeOfficerID { get; set; }

        [ForeignKey(nameof(RangeOfficerID))]
        public virtual RangeOfficer? RangeOfficer { get; set; }

        [StringLength(100)]
        public string? Location { get; set; }

        [StringLength(50)]
        public string? EventType { get; set; }

        public string? Notes { get; set; }

        public int? LastUserID { get; set; }

        [ForeignKey(nameof(LastUserID))]
        public virtual User? LastUser { get; set; }

        public DateTime LastUpdated { get; set; } = DateTime.UtcNow;

        public virtual ICollection<RangeStatistic> RangeStatistics { get; set; } = new List<RangeStatistic>();
    }
}

