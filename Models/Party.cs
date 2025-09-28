using System.ComponentModel.DataAnnotations;

namespace InvoiceManagementSystem.Models
{
    public class Party
    {
        [Key]
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? Address { get; set; }

        public List<Invoice> Invoices { get; set; } = new List<Invoice>();
    }
}
