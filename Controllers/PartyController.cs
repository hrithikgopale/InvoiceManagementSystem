using InvoiceManagementSystem.Data;
using InvoiceManagementSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InvoiceManagementSystem.Controllers
{
    public class PartyController : Controller
    {
        private readonly ApplicationDbContext _context;
        public PartyController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult Manage()
        {
            return View();
        }

        // GET: Get all parties
        public async Task<JsonResult> GetAll()
        {
            var parties = await _context.Parties.ToListAsync();
            return Json(parties);
        }

        // POST: Create party
        [HttpPost]
        public async Task<JsonResult> CreateAjax([FromForm] Party party)
        {
            if (!ModelState.IsValid)
                return Json(new { success = false });

            _context.Parties.Add(party);
            await _context.SaveChangesAsync();

            return Json(new
            {
                success = true,
                id = party.Id,
                name = party.Name,
                email = party.Email,
                phone = party.Phone,
                address = party.Address
            });
        }

        // POST: Edit party
        [HttpPost]
        public async Task<JsonResult> EditAjax(int id, [FromForm] Party party)
        {
            var existing = await _context.Parties.FindAsync(id);
            if (existing == null) return Json(new { success = false, message = "Party not found" });

            existing.Name = party.Name;
            existing.Email = party.Email;
            existing.Phone = party.Phone;
            existing.Address = party.Address;

            _context.Parties.Update(existing);
            await _context.SaveChangesAsync();

            return Json(new
            {
                success = true,
                id = existing.Id,
                name = existing.Name,
                email = existing.Email,
                phone = existing.Phone,
                address = existing.Address
            });
        }

        // POST: Delete party
        [HttpPost]
        public async Task<JsonResult> DeleteAjax(int id)
        {
            var existing = await _context.Parties.FindAsync(id);
            if (existing == null) return Json(new { success = false, message = "Party not found" });

            _context.Parties.Remove(existing);
            await _context.SaveChangesAsync();
            return Json(new { success = true });
        }
    }
}
