using InvoiceManagementSystem.Data;
using InvoiceManagementSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InvoiceManagementSystem.Controllers
{
    public class InventoryController : Controller
    {
        private readonly ApplicationDbContext _context;
        public InventoryController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Inventory Manage page
        public IActionResult Manage()
        {
            return View();
        }

        // GET: Get all inventory items
        public async Task<JsonResult> GetAll()
        {
            var items = await _context.InventoryItems.ToListAsync();
            return Json(items);
        }

        // POST: Create inventory item
        [HttpPost]
        public async Task<JsonResult> CreateAjax([FromForm] InventoryItem item)
        {
            if (!ModelState.IsValid)
                return Json(new { success = false });

            _context.InventoryItems.Add(item);
            await _context.SaveChangesAsync();

            return Json(new
            {
                success = true,
                id = item.Id,
                name = item.Name,
                unitPrice = item.UnitPrice,
                stockQuantity = item.StockQuantity
            });
        }

        // POST: Edit inventory item
        [HttpPost]
        public async Task<JsonResult> EditAjax(int id, [FromForm] InventoryItem item)
        {
            var existing = await _context.InventoryItems.FindAsync(id);
            if (existing == null) return Json(new { success = false, message = "Item not found" });

            existing.Name = item.Name;
            existing.Description = item.Description;
            existing.UnitPrice = item.UnitPrice;
            existing.StockQuantity = item.StockQuantity;

            _context.InventoryItems.Update(existing);
            await _context.SaveChangesAsync();

            return Json(new
            {
                success = true,
                id = existing.Id,
                name = existing.Name,
                unitPrice = existing.UnitPrice,
                stockQuantity = existing.StockQuantity
            });
        }

        // POST: Delete inventory item
        [HttpPost]
        public async Task<JsonResult> DeleteAjax(int id)
        {
            var existing = await _context.InventoryItems.FindAsync(id);
            if (existing == null) return Json(new { success = false, message = "Item not found" });

            _context.InventoryItems.Remove(existing);
            await _context.SaveChangesAsync();
            return Json(new { success = true });
        }
    }
}
