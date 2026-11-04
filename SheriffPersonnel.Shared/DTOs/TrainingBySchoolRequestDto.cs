using System;

namespace SheriffPersonnel.Shared.DTOs
{
    public class TrainingBySchoolRequestDto
    {
        public string? SchoolName { get; set; }
        public int? TrainingSchoolID { get; set; }
        public int? StatusID { get; set; }
        public int? ProgramID { get; set; }
        public int? UnitID { get; set; }
        public DateOnly? StartDate { get; set; }
        public DateOnly? EndDate { get; set; }
    }
}

