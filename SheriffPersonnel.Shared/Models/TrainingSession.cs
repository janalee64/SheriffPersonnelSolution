using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SheriffPersonnel.Shared.Models
{
    public class TrainingSession
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int TrainingSessionID { get; set; }

         public int? TrainingSchoolID { get; set; }

        [ForeignKey(nameof(TrainingSchoolID))]
        public virtual TrainingSchool? TrainingSchool { get; set; }

        [StringLength(150)]
        public string? Location { get; set; }

        [StringLength(100)]
        public string? Instructor { get; set; }

        public DateOnly? BeginDate { get; set; }

        public DateOnly? EndDate { get; set; }

        [Column(TypeName = "decimal(5, 2)")]
        public decimal? SessionHours { get; set; }

        public virtual ICollection<EmployeeTraining> EmployeeTrainings { get; set; } = new List<EmployeeTraining>();
    }
}

