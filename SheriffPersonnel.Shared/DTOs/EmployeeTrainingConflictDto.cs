using System;

namespace SheriffPersonnel.Shared.DTOs
{
    public class EmployeeTrainingConflictDto
    {
        public int EmployeeID { get; set; }
        public string EmployeeName { get; set; } = string.Empty;
        public string? BadgeNumber { get; set; }
        public int ConflictingTrainingSessionID { get; set; }
        public string? ConflictingSchoolOrLocation { get; set; }
        public DateOnly? ConflictingBeginDate { get; set; }
        public DateOnly? ConflictingEndDate { get; set; }
        public string ConflictMessage { get; set; } = string.Empty;
    }
}

