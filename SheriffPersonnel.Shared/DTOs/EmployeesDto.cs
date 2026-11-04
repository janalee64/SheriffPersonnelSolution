using System;

namespace SheriffPersonnel.Shared.DTOs
{
    public class EmployeesDto
    {
        public int EmployeeID { get; set; }
        public string? FirstName { get; set; }
        public string? MiddleInitial { get; set; }
        public string? LastName { get; set; }
        public string? SSN { get; set; }
        public string? BadgeNumber { get; set; }
        public string? PTBNumber { get; set; }
        public int? CountyEmpNo { get; set; }
        public DateOnly? DOB { get; set; }
        public DateOnly? HireDate { get; set; }
        public DateOnly? InactiveDate { get; set; }
        public bool IsSheriffEmployee { get; set; }
        public byte[]? EmployeeImage { get; set; }
        public string? WorkEmail { get; set; }
        public string? WorkPhone { get; set; }
        public string? HomePhone { get; set; }
        public string? HomeAddressLine1 { get; set; }
        public int? LastUserID { get; set; }
        public DateTime LastUpdated { get; set; }
        public int? StatusID { get; set; }
        public string? HomeCity { get; set; }
        public string? HomeState { get; set; }
        public string? HomeZipCode { get; set; }
    }
}

