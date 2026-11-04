using System;
using System.ComponentModel.DataAnnotations;

namespace SheriffPersonnel.Shared.DTOs
{
    public class EmployeeDetailDto
    {
        public int EmployeeID { get; set; }

        [Required(ErrorMessage = "First Name is required.")]
        [StringLength(50, ErrorMessage = "First Name cannot exceed 50 characters.")]
        public string? FirstName { get; set; }

        [StringLength(10, ErrorMessage = "Middle Initial cannot exceed 10 characters.")]
        public string? MiddleInitial { get; set; }

        [Required(ErrorMessage = "Last Name is required.")]
        [StringLength(50, ErrorMessage = "Last Name cannot exceed 50 characters.")]
        public string? LastName { get; set; }

        [RegularExpression(@"^(\d{3}-?\d{2}-?\d{4})?$", ErrorMessage = "SSN must be in format XXX-XX-XXXX or 9 digits.")]
        [StringLength(11, ErrorMessage = "SSN cannot exceed 11 characters.")]
        public string? SSN { get; set; }

        [StringLength(20, ErrorMessage = "Badge Number cannot exceed 20 characters.")]
        public string? BadgeNumber { get; set; }

        [StringLength(10, ErrorMessage = "PTB Number cannot exceed 10 characters.")]
        public string? PTBNumber { get; set; }

        public int? CountyEmpNo { get; set; }

        public DateTime? DOB { get; set; }
        public DateTime? HireDate { get; set; }
        public DateTime? InactiveDate { get; set; }

        public bool EmployeeFlag { get; set; } = false;

        public byte[]? EmployeeImage { get; set; }

        [EmailAddress(ErrorMessage = "Invalid email address format.")]
        [StringLength(100, ErrorMessage = "Work Email cannot exceed 100 characters.")]
        public string? WorkEmail { get; set; }

        [Phone(ErrorMessage = "Invalid Work Phone number format.")]
        [StringLength(20, ErrorMessage = "Work Phone cannot exceed 20 characters.")]
        public string? WorkPhone { get; set; }

        [Phone(ErrorMessage = "Invalid Home Phone number format.")]
        [StringLength(20, ErrorMessage = "Home Phone cannot exceed 20 characters.")]
        public string? HomePhone { get; set; }

        [StringLength(150, ErrorMessage = "Address Line 1 cannot exceed 150 characters.")]
        public string? HomeAddressLine1 { get; set; }

        [StringLength(50, ErrorMessage = "City cannot exceed 50 characters.")]
        public string? HomeCity { get; set; }

        [RegularExpression(@"^([A-Za-z]{2})?$", ErrorMessage = "State must be a valid 2-letter state code.")]
        [StringLength(2, ErrorMessage = "State code must be 2 letters.")]
        public string? HomeState { get; set; }

        [RegularExpression(@"^(\d{5}(-\d{4})?)?$", ErrorMessage = "Zip Code must be 5 digits or 5+4 format (e.g. 61602 or 61602-1234).")]
        [StringLength(10, ErrorMessage = "Zip Code cannot exceed 10 characters.")]
        public string? HomeZipCode { get; set; }

        [Required(ErrorMessage = "Status is required.")]
        public int? StatusID { get; set; }

        public string? StatusDescription { get; set; }

        public int? LastUserID { get; set; }
        public DateTime LastUpdated { get; set; } = DateTime.Now;
    }
}
