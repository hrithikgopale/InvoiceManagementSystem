using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InvoiceManagementSystem.Models
{
    public enum FrequencyType
    {
        Daily,
        Weekly,
        Monthly,
        Yearly
    }
    public class RecurringInvoice
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("Invoice")]
        public int InvoiceId { get; set; }
        public DateTime StartDate { get; set; }
        public string? Frequency { get; set; } // e.g., "Daily", "Monthly", "Yearly"
        public bool IsActive { get; set; }

        public Invoice? Invoice { get; set; }
    }
}
