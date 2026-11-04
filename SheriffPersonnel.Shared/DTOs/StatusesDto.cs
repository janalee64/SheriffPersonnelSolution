namespace SheriffPersonnel.Shared.DTOs
{
    public class StatusesDto
    {
        public int StatusID { get; set; }
        public string StatusCode { get; set; } = string.Empty;
        public string? StatusDescription { get; set; }
    }
}

