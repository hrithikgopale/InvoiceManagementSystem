using InvoiceManagementSystem.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InvoiceManagementSystem.Controllers
{
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DashboardController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Dashboard
        public async Task<IActionResult> Index()
        {
            // Total invoices
            var totalInvoices = await _context.Invoices.CountAsync();

            // Total sales
            var totalSales = await _context.Invoices.SumAsync(i => i.TotalAmount);

            // Stock left per item
            var inventory = await _context.InventoryItems
                .Select(i => new { i.Name, i.StockQuantity })
                .ToListAsync();

            // Monthly sales for current year
            var monthlySales = await _context.Invoices
                .Where(i => i.InvoiceDate.Year == DateTime.Now.Year)
                .GroupBy(i => i.InvoiceDate.Month)
                .Select(g => new { Month = g.Key, Total = g.Sum(x => x.TotalAmount) })
                .ToListAsync();

            // Sales per party
            var salesByParty = await _context.Invoices
                .Include(i => i.Party)
                .GroupBy(i => i.Party.Name)
                .Select(g => new { Party = g.Key, Total = g.Sum(x => x.TotalAmount) })
                .ToListAsync();

            ViewBag.TotalInvoices = totalInvoices;
            ViewBag.TotalSales = totalSales;
            ViewBag.Inventory = inventory;
            ViewBag.MonthlySales = monthlySales;
            ViewBag.SalesByParty = salesByParty;

            return View();
        }
    }
}
