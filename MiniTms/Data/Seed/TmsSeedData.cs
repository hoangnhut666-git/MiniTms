using MiniTms.Entities;
using MiniTms.Services;

namespace MiniTms.Data.Seed;

/// <summary>
/// Single source of truth for TMS master seed data (mirrors docs/seed.sql).
/// Used by EF <see cref="ModelBuilder"/> HasData and runtime <see cref="IDataSeeder"/>.
/// </summary>
public static class TmsSeedData
{
    public static readonly DateTime SeedTimestamp = new(2026, 5, 23, 0, 0, 0, DateTimeKind.Unspecified);
    public static readonly DateOnly SeedOrderDate = new(2026, 5, 23);

    public static IReadOnlyList<Customer> Customers { get; } =
    [
        CreateCustomer(1, "Cafe Highland Tây Hồ", "Tây Hồ", 21.0700, 105.8200, true),
        CreateCustomer(2, "Tiệm cơm Đống Đa", "Đống Đa", 21.0150, 105.8300, false),
        CreateCustomer(3, "Văn phòng Long Biên", "Long Biên", 21.0400, 105.9000, false),
        CreateCustomer(4, "Trường THCS Hà Đông", "Hà Đông", 20.9700, 105.7600, false),
        CreateCustomer(5, "Khách sạn Hoàn Kiếm", "Hoàn Kiếm", 21.0290, 105.8520, true),
        CreateCustomer(6, "Quán phở Ba Đình", "Ba Đình", 21.0350, 105.8100, false),
        CreateCustomer(7, "Trung tâm Thanh Xuân", "Thanh Xuân", 20.9950, 105.7950, false),
        CreateCustomer(8, "Siêu thị Hai Bà Trưng", "Hai Bà Trưng", 21.0050, 105.8650, false),
    ];

    public static IReadOnlyList<Vehicle> Vehicles { get; } =
    [
        CreateVehicle(1, "29C-111.11", "V1", "1.5T", 1500, true),
        CreateVehicle(2, "29C-222.22", "V1", "2.5T", 2500, true),
        CreateVehicle(3, "29C-333.33", "V2", "5T", 5000, true),
        CreateVehicle(4, "29C-444.44", "V2", "2.5T", 2500, false),
    ];

    public static IReadOnlyList<RateCard> RateCards { get; } =
    [
        CreateRateCard(1, "V1", "Tây Hồ", "1.5T", 250000, 30000),
        CreateRateCard(2, "V1", "Tây Hồ", "2.5T", 320000, 30000),
        CreateRateCard(3, "V1", "Tây Hồ", "5T", 480000, 30000),
        CreateRateCard(4, "V1", "Đống Đa", "1.5T", 200000, 25000),
        CreateRateCard(5, "V1", "Đống Đa", "2.5T", 270000, 25000),
        CreateRateCard(6, "V1", "Long Biên", "2.5T", 350000, 35000),
        CreateRateCard(7, "V1", "Long Biên", "5T", 520000, 35000),
        CreateRateCard(8, "V1", "Hà Đông", "2.5T", 380000, 40000),
        CreateRateCard(9, "V1", "Hà Đông", "5T", 550000, 40000),
        CreateRateCard(10, "V1", "Hoàn Kiếm", "1.5T", 220000, 25000),
        CreateRateCard(11, "V1", "Ba Đình", "1.5T", 230000, 25000),
        CreateRateCard(12, "V1", "Thanh Xuân", "2.5T", 290000, 30000),
        CreateRateCard(13, "V1", "Thanh Xuân", "5T", 450000, 30000),
        CreateRateCard(14, "V1", "Hai Bà Trưng", "1.5T", 240000, 25000),
        CreateRateCard(15, "V2", "Tây Hồ", "5T", 460000, 30000),
        CreateRateCard(16, "V2", "Long Biên", "5T", 500000, 35000),
    ];

    public static IReadOnlyList<Order> Orders { get; } =
    [
        CreateOrder(1001, 1, 120, new TimeOnly(7, 0), new TimeOnly(9, 0)),
        CreateOrder(1002, 2, 80, new TimeOnly(7, 30), new TimeOnly(10, 0)),
        CreateOrder(1003, 3, 450, new TimeOnly(8, 0), new TimeOnly(11, 0)),
        CreateOrder(1004, 4, 300, new TimeOnly(7, 0), new TimeOnly(10, 0)),
        CreateOrder(1005, 5, 60, new TimeOnly(7, 0), new TimeOnly(8, 30)),
        CreateOrder(1006, 6, 200, new TimeOnly(7, 30), new TimeOnly(10, 30)),
        CreateOrder(1007, 7, 900, new TimeOnly(8, 0), new TimeOnly(11, 0)),
        CreateOrder(1008, 8, 150, new TimeOnly(7, 0), new TimeOnly(9, 30)),
    ];

    private static Customer CreateCustomer(
        int id, string name, string district, double lat, double lng, bool isVip) =>
        new()
        {
            CustomerId = id,
            Name = name,
            District = district,
            Latitude = lat,
            Longitude = lng,
            IsVip = isVip,
            CreatedAt = SeedTimestamp,
            CreatedBy = AuditUsers.Seed,
            IsDeleted = false,
        };

    private static Vehicle CreateVehicle(
        int id, string plate, string vendorCode, string vehicleType, double capacityKg, bool isActive) =>
        new()
        {
            VehicleId = id,
            Plate = plate,
            VendorCode = vendorCode,
            VehicleType = vehicleType,
            CapacityKg = capacityKg,
            IsActive = isActive,
            CreatedAt = SeedTimestamp,
            CreatedBy = AuditUsers.Seed,
            IsDeleted = false,
        };

    private static RateCard CreateRateCard(
        int id, string vendorCode, string toDistrict, string vehicleType, decimal baseCost, decimal dropFee) =>
        new()
        {
            RateCardId = id,
            VendorCode = vendorCode,
            ToDistrict = toDistrict,
            VehicleType = vehicleType,
            BaseCost = baseCost,
            DropFee = dropFee,
            IsActive = true,
            CreatedAt = SeedTimestamp,
            CreatedBy = AuditUsers.Seed,
            IsDeleted = false,
        };

    private static Order CreateOrder(
        int orderId, int customerId, double weightKg, TimeOnly deliveryStart, TimeOnly deliveryEnd) =>
        new()
        {
            OrderId = orderId,
            CustomerId = customerId,
            WeightKg = weightKg,
            OrderDate = SeedOrderDate,
            DeliveryStart = deliveryStart,
            DeliveryEnd = deliveryEnd,
            Status = OrderStatus.New,
            CreatedAt = SeedTimestamp,
            CreatedBy = AuditUsers.Seed,
            IsDeleted = false,
        };
}
