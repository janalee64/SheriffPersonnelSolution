using System;

namespace SheriffPersonnel.Shared.DTOs
{
    public class InventoryItemsDto
    {
        public int InventoryID { get; set; }
        public string? SKU { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public int? CategoryID { get; set; }
        public string? CategoryName { get; set; }
        public int? SupplierID { get; set; }
        public string? SupplierName { get; set; }
        public int? LocationID { get; set; }
        public string? LocationName { get; set; }
        public string? BrandName { get; set; }
        public string? Size { get; set; }
        public string? Color { get; set; }
        public decimal? UnitPrice { get; set; }
        public int? ReorderPoint { get; set; }
        public int? StockQuantity { get; set; }
        public DateOnly? LastPurchaseDate { get; set; }
        public int? LastUserID { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime LastUpdated { get; set; }
    }
}


