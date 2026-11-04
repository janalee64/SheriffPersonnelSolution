using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SheriffPersonnel.Shared.Models
{
    public class EmployeeChange
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int HistoryID { get; set; }

        public int EmployeeID { get; set; }

        [ForeignKey(nameof(EmployeeID))]
        public virtual Employee? Employee { get; set; }

        public DateTime HistoryDate { get; set; } = DateTime.Now;

        public int? ProgramID { get; set; }

        [ForeignKey(nameof(ProgramID))]
        public virtual Program? Program { get; set; }

        public int? LastUserID { get; set; }

        [ForeignKey(nameof(LastUserID))]
        public virtual User? LastUser { get; set; }

        public DateTime LastUpdated { get; set; } = DateTime.Now;

        public int? RankID { get; set; }

        [ForeignKey(nameof(RankID))]
        public virtual Rank? Rank { get; set; }

        public int? UnitID { get; set; }

        [ForeignKey(nameof(UnitID))]
        public virtual Unit? Unit { get; set; }
    }
}

