namespace MiniTms.Entities;

public class Vehicle : AuditEntity
{
    public int VehicleId { get; set; }
    public string Plate { get; set; } = string.Empty;
    public string VendorCode { get; set; } = string.Empty;
    public string VehicleType { get; set; } = string.Empty;
    public double CapacityKg { get; set; }
    public bool IsActive { get; set; } = true;

    public ICollection<Trip> Trips { get; set; } = [];
}
