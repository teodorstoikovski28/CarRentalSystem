using CarRentalSystem.Data;
using CarRentalSystem.Models;
using Microsoft.AspNetCore.Mvc;

namespace CarRentalSystem.Controllers
{
    public class CarsController : Controller
    {
        private readonly ApplicationDbContext context;

        public CarsController(ApplicationDbContext context)
        {
            this.context = context;
        }

        public IActionResult Index()
        {
            var cars = context.Cars.ToList();

            return View(cars);
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
            return View();
        }

        [HttpPost]
        public IActionResult Create(Car car)
        {
            if (!ModelState.IsValid)
            {
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

            return View(car);
        }

        [HttpPost]
        public IActionResult Edit(Car car)
        {
            if (!ModelState.IsValid)
            {
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