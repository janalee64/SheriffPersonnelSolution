using System;
using System.Collections.Generic;

namespace SheriffPersonnel.Shared.DTOs
{
    public class MultipleEquipmentAssignmentResultDto
    {
        public List<int> AssignedEmployeeIDs { get; set; } = new();
        public List<int> AssignedInventoryIDs { get; set; } = new();
        public int TotalEmployeesAssigned => AssignedEmployeeIDs.Count;
        public int TotalItemsAssigned => AssignedInventoryIDs.Count;
        public int TotalAssignmentsCreated { get; set; }
    }
}

