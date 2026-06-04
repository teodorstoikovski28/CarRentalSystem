using Microsoft.AspNetCore.Mvc;

namespace CarRentalSystem.Controllers
{
    public class CategoriesController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
