using System;

namespace SheriffPersonnel.Shared.DTOs
{
    public class FirearmsDto
    {
        public int FirearmID { get; set; }
        public string SerialNumber { get; set; } = string.Empty;
        public string ModelNumber { get; set; } = string.Empty;
        public string Caliber { get; set; } = string.Empty;
        public bool Optic { get; set; }
        public int EmployeeID { get; set; }
        public string? EmployeeName { get; set; }
        public int? EmployeeStatusID { get; set; }
        public string? EmployeeStatus { get; set; }
        public string? Notes { get; set; }
        public int? LastUserID { get; set; }
        public string? LastUserName { get; set; }
        public DateTime LastUpdated { get; set; } = DateTime.Now;
        public int StatusID { get; set; }
        public string? StatusDescription { get; set; }
        public int? MakeID { get; set; }
        public string? MakeName { get; set; }
    }
}
