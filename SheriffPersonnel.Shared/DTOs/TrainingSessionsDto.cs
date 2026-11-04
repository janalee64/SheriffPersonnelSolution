using System;

namespace SheriffPersonnel.Shared.DTOs
{
    public class TrainingSessionsDto
    {
        public int TrainingSessionID { get; set; }
        public int? TrainingSchoolID { get; set; }
        public string? SchoolName { get; set; }
        public string? Location { get; set; }
        public string? Instructor { get; set; }
        public DateOnly? BeginDate { get; set; }
        public DateOnly? EndDate { get; set; }
        public decimal? SessionHours { get; set; }
    }
}
