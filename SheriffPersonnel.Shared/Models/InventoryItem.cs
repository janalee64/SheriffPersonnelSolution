using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SheriffPersonnel.Shared.Models
{
    public class InventoryItem
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int InventoryID { get; set; }

        [StringLength(20)]
        public string? SKU { get; set; }

        [Required]
        [StringLength(100)]
        public string ProductName { get; set; } = string.Empty;

        public int? CategoryID { get; set; }

        [ForeignKey(nameof(CategoryID))]
        public virtual Category? Category { get; set; }

        public int? SupplierID { get; set; }

        [ForeignKey(nameof(SupplierID))]
        public virtual Supplier? Supplier { get; set; }

        public int? LocationID { get; set; }

        [ForeignKey(nameof(LocationID))]
        public virtual Location? Location { get; set; }

        [StringLength(50)]
        public string? BrandName { get; set; }

        [StringLength(20)]
        public string? Size { get; set; }

        [StringLength(30)]
        public string? Color { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal? UnitPrice { get; set; }

        public int? ReorderPoint { get; set; }

        public int? StockQuantity { get; set; }

        public DateOnly? LastPurchaseDate { get; set; }

        public int? LastUserID { get; set; }

        public bool? IsActive { get; set; } = true;

        [ForeignKey(nameof(LastUserID))]
        public virtual User? LastUser { get; set; }

        public DateTime LastUpdated { get; set; } = DateTime.Now;

        public virtual ICollection<EmployeeInventoryAssignment> EmployeeInventoryAssignments { get; set; } = new List<EmployeeInventoryAssignment>();
    }
}

