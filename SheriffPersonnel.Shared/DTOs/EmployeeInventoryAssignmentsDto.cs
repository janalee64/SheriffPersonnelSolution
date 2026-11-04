using System;

namespace SheriffPersonnel.Shared.DTOs
{
    public class EmployeeInventoryAssignmentsDto
    {
        public int EmpInventoryID { get; set; }
        public int EmployeeID { get; set; }
        public string? EmployeeName { get; set; }
        public string? ProgramName { get; set; }
        public string? EmployeeStatus { get; set; }
        public int InventoryID { get; set; }
        public string? ProductName { get; set; }
        public string? CategoryName { get; set; }
        public DateTime IssueDate { get; set; } = DateTime.Now;
        public int Quantity { get; set; } = 1;
        public string? SerialNumber { get; set; }
        public string? Notes { get; set; }
        public DateTime? ReturnDate { get; set; }
        public DateTime? ExpireDate { get; set; }
        public int? LastUserID { get; set; }
        public string? LastUserName { get; set; }
        public DateTime LastUpdated { get; set; } = DateTime.Now;
    }
}


