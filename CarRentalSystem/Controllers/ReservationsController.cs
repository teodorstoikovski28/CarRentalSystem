using System.Security.Claims;
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
                .Include(r => r.User)
                .AsQueryable();

            if (!User.IsInRole("Admin"))
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                reservations = reservations
                    .Where(r => r.UserId == userId);
            }

            return View(reservations.ToList());
        }

        public IActionResult Details(int id)
        {
            var reservation = context.Reservations
                .Include(r => r.Car)
                .Include(r => r.User)
                .FirstOrDefault(r => r.Id == id);

            if (reservation == null)
            {
                return NotFound();
            }

            if (!User.IsInRole("Admin"))
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                if (reservation.UserId != userId)
                {
                    return Unauthorized();
                }
            }

            return View(reservation);
        }

        public IActionResult Create(int? carId)
        {
            if (!carId.HasValue)
            {
                return RedirectToAction("Index", "Cars");
            }

            var car = context.Cars.FirstOrDefault(c => c.Id == carId.Value);

            if (car == null)
            {
                return NotFound();
            }

            var reservation = new Reservation
            {
                CarId = car.Id
            };

            ViewBag.CarName = $"{car.Brand} {car.Model}";
            ViewBag.CarImage = car.ImageUrl;
            ViewBag.PricePerDay = car.PricePerDay;

            ViewBag.ReservedPeriods = context.Reservations
                .Where(r => r.CarId == car.Id)
                .OrderBy(r => r.StartDate)
                .ToList();

            return View(reservation);
        }

        [HttpPost]
        public IActionResult Create(Reservation reservation)
        {
            if (reservation.StartDate.Date < DateTime.Today)
            {
                ModelState.AddModelError("", "Start date cannot be before today.");
            }

            if (reservation.EndDate.Date < reservation.StartDate.Date)
            {
                ModelState.AddModelError("", "End date cannot be before start date.");
            }

            bool overlapExists = context.Reservations.Any(r =>
                r.CarId == reservation.CarId &&
                reservation.StartDate <= r.EndDate &&
                reservation.EndDate >= r.StartDate);

            if (overlapExists)
            {
                ModelState.AddModelError("", "This car is already reserved for the selected period.");
            }

            var car = context.Cars.FirstOrDefault(c => c.Id == reservation.CarId);

            if (!ModelState.IsValid)
            {
                if (car != null)
                {
                    ViewBag.CarName = $"{car.Brand} {car.Model}";
                    ViewBag.CarImage = car.ImageUrl;

                    ViewBag.ReservedPeriods = context.Reservations
                        .Where(r => r.CarId == car.Id)
                        .OrderBy(r => r.StartDate)
                        .ToList();
                }

                return View(reservation);
            }

            reservation.UserId =
                User.FindFirstValue(ClaimTypes.NameIdentifier)!;

            if (car != null)
            {
                var days = (reservation.EndDate - reservation.StartDate).Days + 1;

                if (days < 1)
                {
                    days = 1;
                }

                reservation.TotalPrice = days * car.PricePerDay;
            }

            context.Reservations.Add(reservation);
            context.SaveChanges();

            var payment = new Payment
            {
                ReservationId = reservation.Id,
                PaymentMethod = reservation.PaymentMethod,
                Amount = reservation.TotalPrice,
                PaymentDate = DateTime.Now
            };

            context.Payments.Add(payment);
            context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Edit(int id)
        {
            var reservation = context.Reservations
                .FirstOrDefault(r => r.Id == id);

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
            var existingReservation = context.Reservations
                .FirstOrDefault(r => r.Id == reservation.Id);

            if (existingReservation == null)
            {
                return NotFound();
            }

            existingReservation.CarId = reservation.CarId;
            existingReservation.StartDate = reservation.StartDate;
            existingReservation.EndDate = reservation.EndDate;

            var car = context.Cars.Find(reservation.CarId);

            if (car != null)
            {
                var days = (reservation.EndDate - reservation.StartDate).Days + 1;

                if (days < 1)
                {
                    days = 1;
                }

                existingReservation.TotalPrice =
                    days * car.PricePerDay;
            }

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
            var reservation = context.Reservations
                .Include(r => r.Payment)
                .FirstOrDefault(r => r.Id == id);

            if (reservation == null)
            {
                return NotFound();
            }

            var payment = context.Payments
                .FirstOrDefault(p => p.ReservationId == reservation.Id);

            if (payment != null)
            {
                context.Payments.Remove(payment);
            }

            context.Reservations.Remove(reservation);

            context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }
    }
}