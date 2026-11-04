using System;

namespace SheriffPersonnel.Shared.DTOs
{
    public class TrainingBySchoolDto
    {
        public int EmployeeTrainingID { get; set; }
        public int EmployeeID { get; set; }
        public string EmployeeName { get; set; } = string.Empty;
        public string? EmployeeStatus { get; set; }
        public string? Program { get; set; }
        public string? UnitName { get; set; }
        public DateOnly? BeginDate { get; set; }
        public decimal? Hours { get; set; }
        public string? SchoolName { get; set; }
    }
}

