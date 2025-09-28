using Microsoft.AspNetCore.Mvc;

namespace InvoiceManagementSystem.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
