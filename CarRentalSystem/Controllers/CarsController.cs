using CarRentalSystem.Data;
using CarRentalSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace CarRentalSystem.Controllers
{
    public class CarsController : Controller
    {
        private readonly ApplicationDbContext context;

        public CarsController(ApplicationDbContext context)
        {
            this.context = context;
        }

        public IActionResult Index(string searchTerm, string sortOrder)
        {
            var cars = context.Cars
                .Include(c => c.Category)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                cars = cars.Where(c =>
                    c.Brand.Contains(searchTerm) ||
                    c.Model.Contains(searchTerm));
            }

            switch (sortOrder)
            {
                case "priceAsc":
                    cars = cars.OrderBy(c => c.PricePerDay);
                    break;

                case "priceDesc":
                    cars = cars.OrderByDescending(c => c.PricePerDay);
                    break;

                case "newest":
                    cars = cars.OrderByDescending(c => c.Id);
                    break;

                case "oldest":
                    cars = cars.OrderBy(c => c.Id);
                    break;
            }

            ViewBag.SortOrder = sortOrder;

            return View(cars.ToList());
        }

        public IActionResult Details(int id)
        {
            var car = context.Cars.FirstOrDefault(x => x.Id == id);

            if (car == null)
            {
                return NotFound();
            }

            return View(car);
        }

        public IActionResult Create()
        {
            ViewBag.Categories = context.Categories
                .Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Name
                })
                .ToList();

            return View();
        }

        [HttpPost]
        public IActionResult Create(Car car)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Categories = context.Categories
                    .Select(c => new SelectListItem
                    {
                        Value = c.Id.ToString(),
                        Text = c.Name
                    })
                    .ToList();

                return View(car);
            }

            context.Cars.Add(car);
            context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Edit(int id)
        {
            var car = context.Cars.Find(id);

            if (car == null)
            {
                return NotFound();
            }

            ViewBag.Categories = context.Categories
                .Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Name
                })
                .ToList();

            return View(car);
        }

        [HttpPost]
        public IActionResult Edit(Car car)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Categories = context.Categories
                    .Select(c => new SelectListItem
                    {
                        Value = c.Id.ToString(),
                        Text = c.Name
                    })
                    .ToList();

                return View(car);
            }

            context.Cars.Update(car);
            context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Delete(int id)
        {
            var car = context.Cars.Find(id);

            if (car == null)
            {
                return NotFound();
            }

            return View(car);
        }

        [HttpPost]
        [ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            var car = context.Cars.Find(id);

            if (car == null)
            {
                return NotFound();
            }

            context.Cars.Remove(car);
            context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }
    }
}