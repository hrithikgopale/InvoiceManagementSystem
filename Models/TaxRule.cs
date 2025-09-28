using System.ComponentModel.DataAnnotations;

namespace InvoiceManagementSystem.Models
{
    public class TaxRule
    {
        [Key]
        public int Id { get; set; }
        public string? Name { get; set; } // e.g., "GST", "VAT"
        public decimal Percentage { get; set; } // e.g., 18 for 18%
    }
}
