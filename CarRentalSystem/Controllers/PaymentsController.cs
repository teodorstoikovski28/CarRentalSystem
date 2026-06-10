
using System.Security.Claims;
using CarRentalSystem.Data;
using CarRentalSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace CarRentalSystem.Controllers
{
    public class PaymentsController : Controller
    {
        private readonly ApplicationDbContext context;

        public PaymentsController(ApplicationDbContext context)
        {
            this.context = context;
        }

        public IActionResult Index()
        {
            var payments = context.Payments
                .Include(p => p.Reservation)
                .ThenInclude(r => r.Car)
                .AsQueryable();

            if (!User.IsInRole("Admin"))
            {
                var userId =
                    User.FindFirstValue(ClaimTypes.NameIdentifier);

                payments = payments.Where(p =>
                    p.Reservation!.UserId == userId);
            }

            return View(payments.ToList());
        }

        public IActionResult Create()
        {
            ViewBag.Reservations = context.Reservations
                .Include(r => r.Car)
                .Select(r => new SelectListItem
                {
                    Value = r.Id.ToString(),
                    Text = r.Car!.Brand + " " + r.Car.Model
                })
                .ToList();

            return View();
        }

        [HttpPost]
        public IActionResult Create(Payment payment)
        {
            var reservation = context.Reservations
                .FirstOrDefault(r => r.Id == payment.ReservationId);

            if (reservation != null)
            {
                payment.Amount = reservation.TotalPrice;
            }

            payment.PaymentDate = DateTime.Now;

            context.Payments.Add(payment);
            context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Details(int id)
        {
            var payment = context.Payments
                .Include(p => p.Reservation)
                .ThenInclude(r => r.Car)
                .FirstOrDefault(p => p.Id == id);

            if (payment == null)
            {
                return NotFound();
            }

            if (!User.IsInRole("Admin"))
            {
                var userId =
                    User.FindFirstValue(ClaimTypes.NameIdentifier);

                if (payment.Reservation?.UserId != userId)
                {
                    return Unauthorized();
                }
            }

            return View(payment);
        }

        public IActionResult Delete(int id)
        {
            var payment = context.Payments
                .Include(p => p.Reservation)
                .ThenInclude(r => r.Car)
                .FirstOrDefault(p => p.Id == id);

            if (payment == null)
            {
                return NotFound();
            }

            if (!User.IsInRole("Admin"))
            {
                var userId =
                    User.FindFirstValue(ClaimTypes.NameIdentifier);

                if (payment.Reservation?.UserId != userId)
                {
                    return Unauthorized();
                }
            }

            return View(payment);
        }

        [HttpPost]
        [ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            var payment = context.Payments
                .Include(p => p.Reservation)
                .FirstOrDefault(p => p.Id == id);

            if (payment == null)
            {
                return NotFound();
            }

            if (!User.IsInRole("Admin"))
            {
                var userId =
                    User.FindFirstValue(ClaimTypes.NameIdentifier);

                if (payment.Reservation?.UserId != userId)
                {
                    return Unauthorized();
                }
            }

            context.Payments.Remove(payment);
            context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }
    }
}
