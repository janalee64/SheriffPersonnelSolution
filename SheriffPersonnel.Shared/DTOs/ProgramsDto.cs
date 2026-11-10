using System.ComponentModel.DataAnnotations;

namespace SheriffPersonnel.Shared.DTOs
{
    public class ProgramsDto
    {
        public int ProgramID { get; set; }

        [Required(ErrorMessage = "Program Name is required.")]
        public string ProgramName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Description is required.")]
        public string? Description { get; set; }
        public bool IsActive { get; set; } = true;

        public string DisplayName => string.IsNullOrWhiteSpace(Description) ? ProgramName : $"{ProgramName} - {Description}";
    }
}
