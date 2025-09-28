using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InvoiceManagementSystem.Models
{
    public class InvoiceItem
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("Invoice")]
        public int InvoiceId { get; set; }

        [ForeignKey("InventoryItem")]
        public int InventoryItemId { get; set; }     
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal Total => Quantity * Price;

        public Invoice Invoice { get; set; }
        public InventoryItem InventoryItem { get; set; }
    }
}
