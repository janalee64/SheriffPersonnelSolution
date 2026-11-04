using System;

namespace SheriffPersonnel.Shared.DTOs
{
    public class RangeStatisticDTO
    {
        public int RangeID { get; set; }
        public int EmployeeID { get; set; }
        public string? EmployeeName { get; set; }
        public string? EmployeeStatus { get; set; }
        public int? EmployeeStatusID { get; set; }
        public string? Program { get; set; }
        public int? ProgramID { get; set; }
        public string? Unit { get; set; }
        public int? UnitID { get; set; }
        public int? FirearmID { get; set; }
        public string? FirearmDescription { get; set; }
        public string? SerialNumber { get; set; }
        public int? CourseID { get; set; }
        public string? CourseCode { get; set; }
        public string? CourseName { get; set; }
        public int? RoundsFired { get; set; }
        public string? PassFail { get; set; }
        public int LastUserID { get; set; }
        public string? LastUserName { get; set; }
        public DateTime LastUpdated { get; set; } = DateTime.Now;
        public int RangeEventID { get; set; }
        public DateOnly? RangeDate { get; set; }
        public string? EventDescription { get; set; }
        public List<int> SelectedSkillIDs { get; set; } = new();
    }
}
