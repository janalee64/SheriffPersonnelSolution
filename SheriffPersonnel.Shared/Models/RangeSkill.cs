using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SheriffPersonnel.Shared.Models
{
    [Table("RangeSkills")]
    public class RangeSkill
    {
        [Key]
        [Column("SkillID")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int RangeSkillID { get; set; }

        [Required]
        [Column("RangeSkill")]
        [StringLength(100)]
        public string SkillName { get; set; } = string.Empty;
    }
}

