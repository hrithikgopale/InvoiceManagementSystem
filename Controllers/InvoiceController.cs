using InvoiceManagementSystem.Data;
using InvoiceManagementSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using QuestPDF.Fluent;

namespace InvoiceManagementSystem.Controllers
{
    public class InvoiceController : Controller
    {
        private readonly ApplicationDbContext _context;
        public InvoiceController(ApplicationDbContext context)
        {
            _context = context;
        }
      

        // GET: Invoice
        public async Task<IActionResult> Index()
        {
            var invoices = await _context.Invoices
                .Include(i => i.Party)
                .Include(i => i.Items)
                .ThenInclude(ii => ii.InventoryItem)
                .ToListAsync();

            return View(invoices);
        }

        // GET: Invoice/Create
        public async Task<IActionResult> Create()
        {
            ViewBag.Parties = await _context.Parties.ToListAsync();
            ViewBag.InventoryItems = await _context.InventoryItems.ToListAsync();
            return View();
        }

        // POST: Invoice/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int partyId, List<int> itemIds, List<int> quantities)
        {
            if (partyId == 0 || itemIds.Count == 0)
            {
                ModelState.AddModelError("", "Party and items are required");
                ViewBag.Parties = await _context.Parties.ToListAsync();
                ViewBag.InventoryItems = await _context.InventoryItems.ToListAsync();
                return View();
            }

            var invoice = new Invoice
            {
                InvoiceNumber = "INV" + DateTime.Now.Ticks,
                InvoiceDate = DateTime.Now,
                PartyId = partyId
            };

            decimal totalAmount = 0;

            for (int i = 0; i < itemIds.Count; i++)
            {
                var inventoryItem = await _context.InventoryItems.FindAsync(itemIds[i]);
                if (inventoryItem != null)
                {
                    // 1. Validate stock
                    if (inventoryItem.StockQuantity < quantities[i])
                    {
                        ModelState.AddModelError("", $"Not enough stock for {inventoryItem.Name}. Available: {inventoryItem.StockQuantity}");
                        ViewBag.Parties = await _context.Parties.ToListAsync();
                        ViewBag.InventoryItems = await _context.InventoryItems.ToListAsync();
                        return View();
                    }

                    // 2. Reduce stock
                    inventoryItem.StockQuantity -= quantities[i];

                    // 3. Add invoice item
                    var invoiceItem = new InvoiceItem
                    {
                        InventoryItemId = inventoryItem.Id,
                        Quantity = quantities[i],
                        Price = inventoryItem.UnitPrice
                    };

                    totalAmount += invoiceItem.Total;
                    invoice.Items.Add(invoiceItem);
                }
            }

            invoice.TotalAmount = totalAmount;

            _context.Invoices.Add(invoice);

            // 4. Save changes (Invoice + updated stock)
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Invoice created successfully!";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Details(int id)
        {
            var invoice = await _context.Invoices
                .Include(i => i.Party)
                .Include(i => i.Items)
                .ThenInclude(ii => ii.InventoryItem)
                .FirstOrDefaultAsync(i => i.Id == id);

            if (invoice == null) return NotFound();

            return View(invoice);
        }
        public IActionResult Print(int id)
        {
            QuestPDF.Settings.License = QuestPDF.Infrastructure.LicenseType.Community;

            var invoice = _context.Invoices
                .Include(i => i.Party)
                .Include(i => i.Items)
                .ThenInclude(ii => ii.InventoryItem)
                .FirstOrDefault(i => i.Id == id);

            if (invoice == null)
                return NotFound();

            var pdfBytes = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(2, Unit.Centimetre);

                    // Header
                    page.Header()
                        .Text($"Invoice #{invoice.InvoiceNumber}")
                        .FontSize(20).Bold().AlignCenter();

                    // Content
                    page.Content().Column(col =>
                    {
                        col.Item().Text($"Date: {invoice.InvoiceDate:d}").FontSize(12);
                        col.Item().Text($"Party: {invoice.Party?.Name}").FontSize(12);

                        col.Item().LineHorizontal(1);

                        col.Item().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn(3);
                                columns.RelativeColumn(2);
                                columns.RelativeColumn(1);
                                columns.RelativeColumn(2);
                            });

                            // Table header
                            table.Header(header =>
                            {
                                header.Cell().Text("Item").Bold();
                                header.Cell().Text("Price").Bold();
                                header.Cell().Text("Qty").Bold();
                                header.Cell().Text("Total").Bold();
                            });

                            // Table rows
                            foreach (var item in invoice.Items)
                            {
                                table.Cell().Text(item.InventoryItem?.Name);
                                table.Cell().Text(item.Price.ToString("C"));
                                table.Cell().Text(item.Quantity.ToString());
                                table.Cell().Text(item.Total.ToString("C"));
                            }

                            // Footer row
                            table.Footer(footer =>
                            {
                                footer.Cell().ColumnSpan(3).AlignRight().Text("Grand Total").Bold();
                                footer.Cell().Text(invoice.TotalAmount.ToString("C")).Bold();
                            });
                        });
                    });

                    // Footer
                    page.Footer().AlignCenter().Text("Thank you for your business!");
                });
            }).GeneratePdf();

            return File(pdfBytes, "application/pdf", $"Invoice_{invoice.InvoiceNumber}.pdf");
        }


    }
}
