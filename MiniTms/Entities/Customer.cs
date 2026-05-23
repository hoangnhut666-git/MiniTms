namespace MiniTms.Entities;

public class Customer : AuditEntity
{
    public int CustomerId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string District { get; set; } = string.Empty;
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public bool IsVip { get; set; }

    public ICollection<Order> Orders { get; set; } = [];
}
