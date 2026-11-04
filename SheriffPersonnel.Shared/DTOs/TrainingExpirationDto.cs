using System;

namespace SheriffPersonnel.Shared.DTOs
{
    public class TrainingExpirationDto
    {
        public int EmployeeTrainingID { get; set; }
        public int EmployeeID { get; set; }
        public DateOnly? ExpireDate { get; set; }
        public string SchoolName { get; set; } = string.Empty;
        public string? FirstName { get; set; }
        public string? MiddleName { get; set; }
        public string? LastName { get; set; }
        public string EmployeeName { get; set; } = string.Empty;
    }
}

