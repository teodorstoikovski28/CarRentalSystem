using Microsoft.AspNetCore.Mvc;

namespace CarRentalSystem.Controllers
{
    public class ReviewsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
