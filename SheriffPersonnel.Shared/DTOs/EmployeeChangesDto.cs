using System;
using System.ComponentModel.DataAnnotations;

namespace SheriffPersonnel.Shared.DTOs
{
    public class EmployeeChangesDto
    {
        public int HistoryID { get; set; }
        public int EmployeeID { get; set; }

        [Required(ErrorMessage = "History Date is required.")]
        public DateTime HistoryDate { get; set; } = DateTime.Now;

        [Required(ErrorMessage = "Program is required.")]
        public int? ProgramID { get; set; }
        public string? ProgramName { get; set; }

        public int? LastUserID { get; set; }
        public string? LastUserName { get; set; }
        public DateTime LastUpdated { get; set; } = DateTime.Now;

        [Required(ErrorMessage = "Rank is required.")]
        public int? RankID { get; set; }
        public string? RankName { get; set; }

        [Required(ErrorMessage = "Unit is required.")]
        public int? UnitID { get; set; }
        public string? UnitName { get; set; }
    }
}
