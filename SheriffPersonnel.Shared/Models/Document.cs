using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SheriffPersonnel.Shared.Models
{
    public class Document
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int DocumentID { get; set; }

        [Required]
        public int EmployeeID { get; set; }

        [Required]
        [StringLength(255)]
        public string Title { get; set; } = string.Empty;

        [Required]
        public int DocumentTypeID { get; set; }

        [Required]
        [StringLength(255)]
        public string FileName { get; set; } = string.Empty;

        [Required]
        public long FileSizeBytes { get; set; }

        [Required]
        [StringLength(100)]
        public string MimeType { get; set; } = string.Empty;

        [Required]
        public byte[] FileData { get; set; } = Array.Empty<byte>();

        [Required]
        public int UploadedBy { get; set; }

        public DateTime? UploadDate { get; set; } = DateTime.Now;

        public virtual Employee? Employee { get; set; }
        public virtual DocumentType? DocumentType { get; set; }
        public virtual User? UploadedByUser { get; set; }
    }
}

