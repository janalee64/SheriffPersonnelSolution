using System.ComponentModel.DataAnnotations;

namespace SheriffPersonnel.Shared.DTOs
{
    public class StatusesDto
    {
        public int StatusID { get; set; }

        [Required(ErrorMessage = "Status Code is required.")]
        public string StatusCode { get; set; } = string.Empty;

        [Required(ErrorMessage = "Status Description is required.")]
        public string? StatusDescription { get; set; }
    }
}
