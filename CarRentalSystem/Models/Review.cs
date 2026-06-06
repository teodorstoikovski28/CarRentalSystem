namespace CarRentalSystem.Models;

public class Review
{
    public int Id { get; set; }

    public string Comment { get; set; } = string.Empty;

    public int Rating { get; set; }
}