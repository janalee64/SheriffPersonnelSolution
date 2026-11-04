using System;
using System.Collections.Generic;

namespace SheriffPersonnel.Shared.DTOs
{
    public class MultipleTrainingAssignmentResultDto
    {
        public int TrainingSessionID { get; set; }
        public List<int> AssignedEmployeeIDs { get; set; } = new();
        public List<EmployeeTrainingConflictDto> Conflicts { get; set; } = new();
        public int TotalRequested { get; set; }
        public int TotalAssigned => AssignedEmployeeIDs.Count;
        public int TotalConflicts => Conflicts.Count;
    }
}

