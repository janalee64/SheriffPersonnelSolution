namespace SheriffPersonnel.Shared.DTOs
{
    public class RanksDto
    {
        public int RankID { get; set; }
        public string RankName { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
    }
}

