using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SheriffPersonnel.Shared.Models
{
    public class AuditLog
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long AuditLogID { get; set; }

        [Required]
        [StringLength(128)]
        public string SchemaName { get; set; } = string.Empty;

        [Required]
        [StringLength(128)]
        public string TableName { get; set; } = string.Empty;

        [Required]
        [StringLength(400)]
        public string PrimaryKeyValues { get; set; } = string.Empty;

        [Required]
        [StringLength(10)]
        public string Action { get; set; } = string.Empty;

        [Required]
        [StringLength(256)]
        public string ChangedBy { get; set; } = string.Empty;

        public DateTime ChangedDate { get; set; } = DateTime.UtcNow;

        [Required]
        public string OldValuesJSON { get; set; } = string.Empty;

        public string? NewValuesJSON { get; set; }
    }
}

