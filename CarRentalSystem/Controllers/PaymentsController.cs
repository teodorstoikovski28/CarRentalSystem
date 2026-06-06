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
                .ToList();

            return View(payments);
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

            return View(payment);
        }

        [HttpPost]
        [ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            var payment = context.Payments.Find(id);

            if (payment == null)
            {
                return NotFound();
            }

            context.Payments.Remove(payment);
            context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }
    }
}