namespace MiniTms.Entities;

/// <summary>
/// Planned trip output from the optimizer (append-only operational data).
/// </summary>
public class Trip
{
    public int TripId { get; set; }
    public string PlanCode { get; set; } = string.Empty;
    public int VehicleId { get; set; }
    public double TotalKm { get; set; }
    public decimal TotalCost { get; set; }
    public string Strategy { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }

    public Vehicle? Vehicle { get; set; }
    public ICollection<TripOrder> TripOrders { get; set; } = [];
}
