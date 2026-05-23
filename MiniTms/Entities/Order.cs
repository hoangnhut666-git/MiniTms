namespace MiniTms.Entities;

public class Order : AuditEntity
{
    public int OrderId { get; set; }
    public int CustomerId { get; set; }
    public double WeightKg { get; set; }
    public DateOnly OrderDate { get; set; }
    public TimeOnly DeliveryStart { get; set; }
    public TimeOnly DeliveryEnd { get; set; }
    public string Status { get; set; } = OrderStatus.New;

    public Customer Customer { get; set; } = null!;
    public TripOrder? TripOrder { get; set; }
}
