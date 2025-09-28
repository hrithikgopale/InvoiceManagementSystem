using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InvoiceManagementSystem.Models
{
    public class RecurringInvoiceItem
    {
        [Key]
        public int Id { get; set; }

        // Relation to RecurringInvoice
        [ForeignKey("RecurringInvoice")]
        public int RecurringInvoiceId { get; set; }
       

        // Relation to InventoryItem
        [ForeignKey("InventoryItem")]
        public int InventoryItemId { get; set; }
      

        public decimal Price { get; set; }
        public int Quantity { get; set; }

        // Optional: Calculated total
        [NotMapped]
        public decimal Total => Price * Quantity;

        public RecurringInvoice RecurringInvoice { get; set; } = default!;
        public InventoryItem InventoryItem { get; set; } = default!;

    }
}
