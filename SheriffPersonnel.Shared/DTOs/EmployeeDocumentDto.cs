using System;
using System.ComponentModel.DataAnnotations;

namespace SheriffPersonnel.Shared.DTOs
{
    public class EmployeeDocumentDto
    {
        public int DocumentID { get; set; }

        [Required(ErrorMessage = "Employee is required.")]
        public int EmployeeID { get; set; }

        [Required(ErrorMessage = "Title is required.")]
        [StringLength(255, ErrorMessage = "Title cannot exceed 255 characters.")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Document Type is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Document Type is required.")]
        public int DocumentTypeID { get; set; }

        public string? DocumentTypeName { get; set; }

        public string FileName { get; set; } = string.Empty;

        public long FileSizeBytes { get; set; }

        public string MimeType { get; set; } = string.Empty;

        public int UploadedBy { get; set; }

        public string? UploadedByName { get; set; }

        public DateTime? UploadDate { get; set; }

        public byte[]? FileData { get; set; }
    }
}

