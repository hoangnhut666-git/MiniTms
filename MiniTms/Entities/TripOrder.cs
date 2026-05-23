namespace MiniTms.Entities;

public class TripOrder
{
    public int TripOrderId { get; set; }
    public int TripId { get; set; }
    public int OrderId { get; set; }
    public int StopIndex { get; set; }
    public TimeOnly EtaTime { get; set; }

    public Trip Trip { get; set; } = null!;
    public Order Order { get; set; } = null!;
}
