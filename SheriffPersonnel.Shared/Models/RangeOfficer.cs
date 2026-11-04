using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SheriffPersonnel.Shared.Models
{
    public class RangeOfficer
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int RangeOfficerID { get; set; }

        [Required]
        [StringLength(100)]
        public string OfficerName { get; set; } = string.Empty;

        public bool? IsActive { get; set; } = true;
    }
}

