using System;

namespace SheriffPersonnel.Shared.DTOs
{
    public class EmployeeSearchResultDto
    {
        public int EmployeeID { get; set; }
        public int Id { get => EmployeeID; set => EmployeeID = value; }

        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string BadgeNumber { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public bool EmployeeFlag { get; set; }
        public byte[]? EmployeeImage { get; set; }
        public DateTime? HireDate { get; set; }
        public DateTime? InactiveDate { get; set; }
        public string? Program { get; set; }
        public string? Unit { get; set; }

        public string FormattedName => !string.IsNullOrWhiteSpace(LastName) && !string.IsNullOrWhiteSpace(FirstName)
            ? $"{LastName}, {FirstName}"
            : (!string.IsNullOrWhiteSpace(LastName) ? LastName : FirstName);
    }
}
