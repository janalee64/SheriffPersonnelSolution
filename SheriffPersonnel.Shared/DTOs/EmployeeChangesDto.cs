namespace SheriffPersonnel.Shared.DTOs
{
    public class EmployeeChangesDto
    {
        public int HistoryID { get; set; }
        public int EmployeeID { get; set; }
        public DateTime HistoryDate { get; set; } = DateTime.Now;
        public int? ProgramID { get; set; }
        public string? ProgramName { get; set; }
        public int? LastUserID { get; set; }
        public string? LastUserName { get; set; }
        public DateTime LastUpdated { get; set; } = DateTime.Now;
        public int? RankID { get; set; }
        public string? RankName { get; set; }
        public int? UnitID { get; set; }
        public string? UnitName { get; set; }
    }
}

