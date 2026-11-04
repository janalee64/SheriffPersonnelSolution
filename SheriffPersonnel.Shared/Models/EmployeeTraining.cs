using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SheriffPersonnel.Shared.Models
{
    public class EmployeeTraining
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int EmployeeTrainingID { get; set; }

        public int EmployeeID { get; set; }

        [ForeignKey(nameof(EmployeeID))]
        public virtual Employee? Employee { get; set; }

        public int TrainingSessionID { get; set; }

        [ForeignKey(nameof(TrainingSessionID))]
        public virtual TrainingSession? TrainingSession { get; set; }

        [Column(TypeName = "decimal(5, 2)")]
        public decimal? HoursCompleted { get; set; }

        public DateOnly? ExpireDate { get; set; }

        [StringLength(10)]
        public string? OrderNumber { get; set; }

        [Column(TypeName = "decimal(10, 2)")]
        public decimal RegistrationCost { get; set; }

        [Column(TypeName = "decimal(10, 2)")]
        public decimal TravelCost { get; set; }

        public int LastUserID { get; set; }

        [ForeignKey(nameof(LastUserID))]
        public virtual User LastUser { get; set; } = null!;

        public DateTime LastUpdated { get; set; } = DateTime.Now;
    }
}

