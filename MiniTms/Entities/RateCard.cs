namespace MiniTms.Entities;

public class RateCard : AuditEntity
{
    public int RateCardId { get; set; }
    public string VendorCode { get; set; } = string.Empty;
    public string ToDistrict { get; set; } = string.Empty;
    public string VehicleType { get; set; } = string.Empty;
    public decimal BaseCost { get; set; }
    public decimal DropFee { get; set; }
    public bool IsActive { get; set; } = true;
}
