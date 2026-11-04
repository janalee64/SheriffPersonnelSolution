using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SheriffPersonnel.Shared.Models
{
    public class Make
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int MakeID { get; set; }

        [Required]
        [StringLength(25)]
        public string MakeName { get; set; } = string.Empty;

        public virtual ICollection<Firearm> Firearms { get; set; } = new List<Firearm>();
    }
}

