using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SheriffPersonnel.Shared.Models
{
    public class RangeCourse
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CourseID { get; set; }

        [StringLength(50)]
        public string? CourseCode { get; set; }

        [Required]
        [StringLength(100)]
        public string CourseName { get; set; } = string.Empty;

        [StringLength(255)]
        public string? Notes { get; set; }

        public bool? IsActive { get; set; } = true;

        public virtual ICollection<RangeStatistic> RangeStatistics { get; set; } = new List<RangeStatistic>();
    }
}
