using System;
using System.Collections.Generic;

namespace SheriffPersonnel.Shared.DTOs
{
    public class MultipleEquipmentAssignmentRequestDto
    {
        public List<int> EmployeeIDs { get; set; } = new();
        public List<int> InventoryIDs { get; set; } = new();

        public DateTime IssueDate { get; set; } = DateTime.Now;
        public int Quantity { get; set; } = 1;
        public string? SerialNumber { get; set; }
        public DateTime? ExpireDate { get; set; }
        public DateTime? ReturnDate { get; set; }
        public string? Notes { get; set; }
        public int? LastUserID { get; set; }
    }
}

