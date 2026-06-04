using Microsoft.AspNetCore.Mvc;

namespace CarRentalSystem.Controllers
{
    public class ReservationsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
