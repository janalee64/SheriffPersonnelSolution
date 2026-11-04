using System;

namespace SheriffPersonnel.Shared.DTOs
{
    public class EmployeeTrainingsDto
    {
        public int EmployeeTrainingID { get; set; }
        public int EmployeeID { get; set; }
        public int TrainingSessionID { get; set; }
        public decimal? HoursCompleted { get; set; }
        public DateOnly? ExpireDate { get; set; }
        public string? OrderNumber { get; set; }
        public decimal RegistrationCost { get; set; }
        public decimal TravelCost { get; set; }
        public int LastUserID { get; set; }
        public DateTime LastUpdated { get; set; }

        public int? TrainingSchoolID { get; set; }
        public string? SchoolName { get; set; }
        public string? Location { get; set; }
        public string? Instructor { get; set; }
        public DateOnly? BeginDate { get; set; }
        public DateOnly? EndDate { get; set; }
        public decimal? SessionHours { get; set; }
    }
}
