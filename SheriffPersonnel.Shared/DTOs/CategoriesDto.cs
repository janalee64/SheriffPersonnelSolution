namespace SheriffPersonnel.Shared.DTOs
{
    public class CategoriesDto
    {
        public int CategoryID { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
    }
}

