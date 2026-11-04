using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SheriffPersonnel.Shared.Models
{
    public class DocumentType
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int DocumentTypeID { get; set; }

        [Required]
        [StringLength(100)]
        public string DocumentTypeName { get; set; } = string.Empty;

        public virtual ICollection<Document> Documents { get; set; } = new List<Document>();
    }
}

