using InvoiceManagementSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace InvoiceManagementSystem.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
        }
        public DbSet<Party> Parties { get; set; }
        public DbSet<Invoice> Invoices { get; set; }
        public DbSet<InvoiceItem> InvoiceItems { get; set; }
        public DbSet<InventoryItem> InventoryItems { get; set; }
        public DbSet<RecurringInvoice> RecurringInvoices { get; set; }
        public DbSet<TaxRule> TaxRules { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Party - Invoice (1:N)
            modelBuilder.Entity<Invoice>()
                .HasOne(i => i.Party)
                .WithMany(p => p.Invoices)
                .HasForeignKey(i => i.PartyId)
                .OnDelete(DeleteBehavior.Cascade);

            // Invoice - InvoiceItem (1:N)
            modelBuilder.Entity<InvoiceItem>()
                .HasOne(ii => ii.Invoice)
                .WithMany(i => i.Items)
                .HasForeignKey(ii => ii.InvoiceId)
                .OnDelete(DeleteBehavior.Cascade);

            // InvoiceItem - InventoryItem (N:1)
            modelBuilder.Entity<InvoiceItem>()
                .HasOne(ii => ii.InventoryItem)
                .WithMany(ii => ii.InvoiceItems)
                .HasForeignKey(ii => ii.InventoryItemId)
                .OnDelete(DeleteBehavior.Restrict);

            // Invoice - RecurringInvoice (1:1)
            modelBuilder.Entity<RecurringInvoice>()
                .HasOne(ri => ri.Invoice)
                .WithOne(i => i.RecurringInvoice)
                .HasForeignKey<RecurringInvoice>(ri => ri.InvoiceId)
                .OnDelete(DeleteBehavior.Cascade);


        }

    }
    }
