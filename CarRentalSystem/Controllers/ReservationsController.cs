using CarRentalSystem.Data;
using CarRentalSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace CarRentalSystem.Controllers
{
    public class ReservationsController : Controller
    {
        private readonly ApplicationDbContext context;

        public ReservationsController(ApplicationDbContext context)
        {
            this.context = context;
        }

        public IActionResult Index()
        {
            var reservations = context.Reservations
                .Include(r => r.Car)
                .ToList();

            return View(reservations);
        }

        public IActionResult Details(int id)
        {
            var reservation = context.Reservations
                .Include(r => r.Car)
                .FirstOrDefault(r => r.Id == id);

            if (reservation == null)
            {
                return NotFound();
            }

            return View(reservation);
        }

        public IActionResult Create()
        {
            ViewBag.Cars = context.Cars
                .Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Brand + " " + c.Model
                })
                .ToList();

            return View();
        }

        [HttpPost]
        public IActionResult Create(Reservation reservation)
        {
            var car = context.Cars.Find(reservation.CarId);

            if (car != null)
            {
                var days = (reservation.EndDate - reservation.StartDate).Days;

                if (days < 1)
                {
                    days = 1;
                }

                reservation.TotalPrice = days * car.PricePerDay;
            }

            context.Reservations.Add(reservation);
            context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Edit(int id)
        {
            var reservation = context.Reservations.Find(id);

            if (reservation == null)
            {
                return NotFound();
            }

            ViewBag.Cars = context.Cars
                .Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Brand + " " + c.Model
                })
                .ToList();

            return View(reservation);
        }

        [HttpPost]
        public IActionResult Edit(Reservation reservation)
        {
            var car = context.Cars.Find(reservation.CarId);

            if (car != null)
            {
                var days = (reservation.EndDate - reservation.StartDate).Days;

                if (days < 1)
                {
                    days = 1;
                }

                reservation.TotalPrice = days * car.PricePerDay;
            }

            context.Reservations.Update(reservation);
            context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Delete(int id)
        {
            var reservation = context.Reservations
                .Include(r => r.Car)
                .FirstOrDefault(r => r.Id == id);

            if (reservation == null)
            {
                return NotFound();
            }

            return View(reservation);
        }

        [HttpPost]
        [ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            var reservation = context.Reservations.Find(id);

            if (reservation == null)
            {
                return NotFound();
            }

            context.Reservations.Remove(reservation);
            context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }
    }
}