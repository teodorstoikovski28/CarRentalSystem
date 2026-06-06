using System.ComponentModel.DataAnnotations.Schema;

namespace CarRentalSystem.Models;

public class Reservation
{
    public int Id { get; set; }

    public DateTime StartDate { get; set; } = DateTime.Today;

    public DateTime EndDate { get; set; } = DateTime.Today.AddDays(1);

    public decimal TotalPrice { get; set; }

    public int CarId { get; set; }

    public Car? Car { get; set; }

    public string PaymentMethod { get; set; } = string.Empty;

    public Payment? Payment { get; set; }

    public string UserId { get; set; } = string.Empty;

    [ForeignKey(nameof(UserId))]
    public ApplicationUser? User { get; set; }
}