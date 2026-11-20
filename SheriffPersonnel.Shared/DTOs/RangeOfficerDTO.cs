namespace SheriffPersonnel.Shared.DTOs
{
    public class RangeOfficerDTO
    {
        public int RangeOfficerID { get; set; }
        public string OfficerName { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
        public int? UserID { get; set; }
        public string? UserDisplayName { get; set; }
    }
}


