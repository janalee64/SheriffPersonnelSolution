using System.ComponentModel.DataAnnotations;

namespace SheriffPersonnel.Shared.DTOs
{
    public class RangeCourseDTO
    {
        public int CourseID { get; set; }

        [Required(ErrorMessage = "Course Code is required.")]
        public string? CourseCode { get; set; }

        [Required(ErrorMessage = "Sort Order is required.")]
        public int? SortOrder { get; set; }

        [Required(ErrorMessage = "Course Name is required.")]
        public string CourseName { get; set; } = string.Empty;

        public string? Notes { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;

        public string FormattedCourse => !string.IsNullOrWhiteSpace(CourseCode) && !string.IsNullOrWhiteSpace(CourseName)
            ? $"{CourseCode} - {CourseName}"
            : (!string.IsNullOrWhiteSpace(CourseCode) ? CourseCode : CourseName);
    }
}
