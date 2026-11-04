namespace SheriffPersonnel.Shared.DTOs
{
    public class UnitsDto
    {
        public int UnitID { get; set; }
        public string UnitName { get; set; } = string.Empty;
        public string? Notes { get; set; }
        public bool IsActive { get; set; } = true;
    }
}

