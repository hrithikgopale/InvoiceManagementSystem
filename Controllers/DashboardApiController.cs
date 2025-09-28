using InvoiceManagementSystem.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[Route("api/dashboard")]
[ApiController]
public class DashboardApiController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    public DashboardApiController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet("totals")]
    public async Task<IActionResult> GetTotals()
    {
        var totalInvoices = await _context.Invoices.CountAsync();
        var totalSales = await _context.Invoices.SumAsync(i => i.TotalAmount);

        return Ok(new { totalInvoices, totalSales });
    }

    [HttpGet("inventory")]
    public async Task<IActionResult> GetInventory()
    {
        var inventory = await _context.InventoryItems
            .Select(i => new { i.Name, i.StockQuantity })
            .ToListAsync();
        return Ok(inventory);
    }

    [HttpGet("monthly-sales")]
    public async Task<IActionResult> GetMonthlySales()
    {
        var currentYear = DateTime.Now.Year;
        var monthlySales = await _context.Invoices
            .Where(i => i.InvoiceDate.Year == currentYear)
            .GroupBy(i => i.InvoiceDate.Month)
            .Select(g => new { month = g.Key, total = g.Sum(x => x.TotalAmount) })
            .ToListAsync();

        return Ok(monthlySales);
    }

    [HttpGet("sales-by-party")]
    public async Task<IActionResult> GetSalesByParty()
    {
        var salesByParty = await _context.Invoices
            .Include(i => i.Party)
            .GroupBy(i => i.Party.Name)
            .Select(g => new { party = g.Key, total = g.Sum(x => x.TotalAmount) })
            .ToListAsync();

        return Ok(salesByParty);
    }
}
