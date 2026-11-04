namespace SheriffPersonnel.Shared.DTOs
{
    public class TrainingSchoolsDto
    {
        public int TrainingSchoolID { get; set; }
        public string SchoolName { get; set; } = string.Empty;
        public string? Address { get; set; }
        public bool IsActive { get; set; }
    }
}

