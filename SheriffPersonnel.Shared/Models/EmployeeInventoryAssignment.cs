using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SheriffPersonnel.Shared.Models
{
    public class EmployeeInventoryAssignment
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int EmpInventoryID { get; set; }

        public int EmployeeID { get; set; }

        [ForeignKey(nameof(EmployeeID))]
        public virtual Employee? Employee { get; set; }

        public int InventoryID { get; set; }

        [ForeignKey(nameof(InventoryID))]
        public virtual InventoryItem? InventoryItem { get; set; }

        public DateTime IssueDate { get; set; } = DateTime.Now;

        public int Quantity { get; set; }

        [StringLength(100)]
        public string? SerialNumber { get; set; }

        [StringLength(255)]
        public string? Notes { get; set; }

        public DateTime? ReturnDate { get; set; }

        public DateTime? ExpireDate { get; set; }

        public int? LastUserID { get; set; }

        [ForeignKey(nameof(LastUserID))]
        public virtual User? LastUser { get; set; }

        public DateTime LastUpdated { get; set; } = DateTime.Now;
    }
}

