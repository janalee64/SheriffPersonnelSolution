namespace SheriffPersonnel.Shared.DTOs
{
    public class ProgramsDto
    {
        public int ProgramID { get; set; }
        public string ProgramName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool IsActive { get; set; } = true;

        public string DisplayName => string.IsNullOrWhiteSpace(Description) ? ProgramName : $"{ProgramName} - {Description}";
    }
}

