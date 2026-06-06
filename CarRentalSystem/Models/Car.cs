namespace CarRentalSystem.Models;

public class Car
{
    public int Id { get; set; }

    public string Brand { get; set; } = string.Empty;

    public string Model { get; set; } = string.Empty;

    public int Year { get; set; }

    public decimal PricePerDay { get; set; }

    public string ImageUrl { get; set; } = string.Empty;

    public bool IsAvailable { get; set; }

    public int CategoryId { get; set; }

    public Category? Category { get; set; }

    public ICollection<Reservation> Reservations { get; set; }
        = new List<Reservation>();
}