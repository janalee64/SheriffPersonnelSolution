namespace SheriffPersonnel.Shared.DTOs
{
    public class EmployeeSearchRequestDto
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Status { get; set; }
        public string? BadgeNumber { get; set; }
        public bool? EmployeeFlag { get; set; }

        public int? ProgramID { get; set; }
        public int? UnitID { get; set; }

        public string? SortBy { get; set; }
        public bool SortDescending { get; set; }
    }
}
