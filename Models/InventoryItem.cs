using System.ComponentModel.DataAnnotations;

namespace InvoiceManagementSystem.Models
{
    public class InventoryItem
    {
        [Key]
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public decimal UnitPrice { get; set; }
        public int StockQuantity { get; set; }

        public List<InvoiceItem> InvoiceItems { get; set; } = new List<InvoiceItem>();
    }
}
