using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;

namespace InvoiceManagementSystem.Models
{
    public class Invoice
    {
        [Key]
        public int Id { get; set; }
        public string? InvoiceNumber { get; set; }
        public DateTime InvoiceDate { get; set; }
        = DateTime.Now;
        [ForeignKey("Party")]
        public int PartyId { get; set; }      
        public decimal TotalAmount { get; set; }
        public Party? Party { get; set; }
        public RecurringInvoice? RecurringInvoice { get; set; }
        public List<InvoiceItem> Items { get; set; } = new List<InvoiceItem>();
    }
}
