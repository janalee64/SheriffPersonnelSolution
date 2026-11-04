using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SheriffPersonnel.Shared.Models
{
    public class TrainingSchool
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int TrainingSchoolID { get; set; }

        [Required]
        [StringLength(150)]
        public string SchoolName { get; set; } = string.Empty;

        [StringLength(200)]
        public string? Address { get; set; }

        public bool? IsActive { get; set; } = true;

        public virtual ICollection<TrainingSession> TrainingSessions { get; set; } = new List<TrainingSession>();
    }
}

