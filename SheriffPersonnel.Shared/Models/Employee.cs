using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SheriffPersonnel.Shared.Models
{
    public class Employee
    {
        public int EmployeeID { get; set; }

        public string? FirstName { get; set; }
        public string? MiddleInitial { get; set; }
        public string? LastName { get; set; }
        [StringLength(11)]
        public string? SSN { get; set; }
        public int? BadgeNumber { get; set; }
        [StringLength(10)]
        public string? PTBNumber { get; set; }
        public int? CountyEmpNo { get; set; }
        public DateOnly? DOB { get; set; }
        public DateOnly? HireDate { get; set; }
        public DateOnly? InactiveDate { get; set; }
        public bool EmployeeFlag { get; set; } = false;
        public bool IsSheriffEmployee { get => EmployeeFlag; set => EmployeeFlag = value; }

        public byte[]? EmployeeImage { get; set; }

        public string? WorkEmail { get; set; }
        public string? WorkPhone { get; set; }
        public string? HomePhone { get; set; }
        public string? HomeAddressLine1 { get; set; }
        public int? LastUserID { get; set; }
        public virtual User? LastUser { get; set; }
        public DateTime LastUpdated { get; set; } = DateTime.Now;

        public int? StatusID { get; set; }
        public virtual Status? Status { get; set; }

        public string? HomeCity { get; set; }
        public string? HomeState { get; set; }
        public string? HomeZipCode { get; set; }

        public virtual ICollection<EmployeeChange> EmployeeChanges { get; set; } = new List<EmployeeChange>();
        public virtual ICollection<EmployeeInventoryAssignment> EmployeeInventoryAssignments { get; set; } = new List<EmployeeInventoryAssignment>();
        public virtual ICollection<Firearm> Firearms { get; set; } = new List<Firearm>();
        public virtual ICollection<RangeStatistic> RangeStatistics { get; set; } = new List<RangeStatistic>();
        public virtual ICollection<EmployeeTraining> EmployeeTrainings { get; set; } = new List<EmployeeTraining>();
        public virtual ICollection<Document> Documents { get; set; } = new List<Document>();
    }
}
