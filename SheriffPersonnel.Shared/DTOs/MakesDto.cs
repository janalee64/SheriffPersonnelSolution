using System.ComponentModel.DataAnnotations;

namespace SheriffPersonnel.Shared.DTOs
{
    public class MakesDto
    {
        public int MakeID { get; set; }

        [Required]
        [StringLength(25, ErrorMessage = "Make Name cannot exceed 25 characters.")]
        public string MakeName { get; set; } = string.Empty;
    }
}

