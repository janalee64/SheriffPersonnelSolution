using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SheriffPersonnel.Shared.Models
{
    public class RangeStatistic
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int RangeID { get; set; }

        public int EmployeeID { get; set; }

        [ForeignKey(nameof(EmployeeID))]
        public virtual Employee Employee { get; set; } = null!;

        public int? FirearmID { get; set; }

        [ForeignKey(nameof(FirearmID))]
        public virtual Firearm? Firearm { get; set; }

        public int? CourseID { get; set; }

        [ForeignKey(nameof(CourseID))]
        public virtual RangeCourse? Course { get; set; }

        public int? RoundsFired { get; set; }

        [StringLength(10)]
        public string? PassFail { get; set; }

        public int LastUserID { get; set; }

        [ForeignKey(nameof(LastUserID))]
        public virtual User LastUser { get; set; } = null!;

        public DateTime LastUpdated { get; set; } = DateTime.Now;

        public int RangeEventID { get; set; }

        [ForeignKey(nameof(RangeEventID))]
        public virtual RangeEvent? RangeEvent { get; set; }

        public virtual ICollection<RangeSkill> RangeSkills { get; set; } = new List<RangeSkill>();
    }
}

