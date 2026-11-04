using System;
using System.Collections.Generic;

namespace SheriffPersonnel.Shared.DTOs
{
    public class MultipleTrainingAssignmentRequestDto
    {
        public List<int> EmployeeIDs { get; set; } = new();

        public int TrainingSessionID { get; set; }
        public int? TrainingSchoolID { get; set; }
        public string? Location { get; set; }
        public string? Instructor { get; set; }
        public DateOnly? BeginDate { get; set; }
        public DateOnly? EndDate { get; set; }
        public decimal? SessionHours { get; set; }

        public decimal? HoursCompleted { get; set; }
        public DateOnly? ExpireDate { get; set; }
        public string? OrderNumber { get; set; }
        public decimal RegistrationCost { get; set; }
        public decimal TravelCost { get; set; }
        public int LastUserID { get; set; }
    }
}

