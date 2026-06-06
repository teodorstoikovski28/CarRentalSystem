using Microsoft.AspNetCore.Mvc;

namespace CarRentalSystem.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return RedirectToAction("Index", "Cars");
        }

        public IActionResult Privacy()
        {
            return View();
        }
    }
}