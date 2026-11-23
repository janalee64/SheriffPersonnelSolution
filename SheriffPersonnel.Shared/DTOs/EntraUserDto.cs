namespace SheriffPersonnel.Shared.DTOs
{
    public class EntraUserDto
    {
        public string Id { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public string UserPrincipalName { get; set; } = string.Empty;
        public string? Mail { get; set; }
        public string? JobTitle { get; set; }
        public string? Department { get; set; }
    }
}
