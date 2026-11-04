namespace SheriffPersonnel.Shared.DTOs
{
    public class RangeCourseDTO
    {
        public int CourseID { get; set; }
        public string? CourseCode { get; set; }
        public string CourseName { get; set; } = string.Empty;
        public string? Notes { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
    }
}
