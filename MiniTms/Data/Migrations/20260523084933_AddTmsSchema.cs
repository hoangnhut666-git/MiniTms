using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace MiniTms.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddTmsSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Customers",
                columns: table => new
                {
                    CustomerId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    District = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Latitude = table.Column<double>(type: "float", nullable: false),
                    Longitude = table.Column<double>(type: "float", nullable: false),
                    IsVip = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Customers", x => x.CustomerId);
                });

            migrationBuilder.CreateTable(
                name: "RateCard",
                columns: table => new
                {
                    RateCardId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    VendorCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ToDistrict = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    VehicleType = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    BaseCost = table.Column<decimal>(type: "decimal(12,0)", nullable: false),
                    DropFee = table.Column<decimal>(type: "decimal(12,0)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RateCard", x => x.RateCardId);
                });

            migrationBuilder.CreateTable(
                name: "Vehicles",
                columns: table => new
                {
                    VehicleId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Plate = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    VendorCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    VehicleType = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    CapacityKg = table.Column<double>(type: "float", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Vehicles", x => x.VehicleId);
                });

            migrationBuilder.CreateTable(
                name: "Orders",
                columns: table => new
                {
                    OrderId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CustomerId = table.Column<int>(type: "int", nullable: false),
                    WeightKg = table.Column<double>(type: "float", nullable: false),
                    OrderDate = table.Column<DateOnly>(type: "date", nullable: false),
                    DeliveryStart = table.Column<TimeOnly>(type: "time", nullable: false),
                    DeliveryEnd = table.Column<TimeOnly>(type: "time", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, defaultValue: "NEW"),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Orders", x => x.OrderId);
                    table.ForeignKey(
                        name: "FK_Orders_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "CustomerId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Trips",
                columns: table => new
                {
                    TripId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PlanCode = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    VehicleId = table.Column<int>(type: "int", nullable: false),
                    TotalKm = table.Column<double>(type: "float", nullable: false),
                    TotalCost = table.Column<decimal>(type: "decimal(12,0)", nullable: false),
                    Strategy = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Trips", x => x.TripId);
                    table.ForeignKey(
                        name: "FK_Trips_Vehicles_VehicleId",
                        column: x => x.VehicleId,
                        principalTable: "Vehicles",
                        principalColumn: "VehicleId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TripOrders",
                columns: table => new
                {
                    TripOrderId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TripId = table.Column<int>(type: "int", nullable: false),
                    OrderId = table.Column<int>(type: "int", nullable: false),
                    StopIndex = table.Column<int>(type: "int", nullable: false),
                    EtaTime = table.Column<TimeOnly>(type: "time", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TripOrders", x => x.TripOrderId);
                    table.ForeignKey(
                        name: "FK_TripOrders_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "OrderId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TripOrders_Trips_TripId",
                        column: x => x.TripId,
                        principalTable: "Trips",
                        principalColumn: "TripId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Customers",
                columns: new[] { "CustomerId", "CreatedAt", "CreatedBy", "DeletedAt", "DeletedBy", "District", "IsDeleted", "IsVip", "Latitude", "Longitude", "Name", "UpdatedAt", "UpdatedBy" },
                values: new object[,]
                {
                    { 1, new DateTime(2026, 5, 23, 0, 0, 0, 0, DateTimeKind.Unspecified), "seed", null, null, "Tây Hồ", false, true, 21.07, 105.81999999999999, "Cafe Highland Tây Hồ", null, null },
                    { 2, new DateTime(2026, 5, 23, 0, 0, 0, 0, DateTimeKind.Unspecified), "seed", null, null, "Đống Đa", false, false, 21.015000000000001, 105.83, "Tiệm cơm Đống Đa", null, null },
                    { 3, new DateTime(2026, 5, 23, 0, 0, 0, 0, DateTimeKind.Unspecified), "seed", null, null, "Long Biên", false, false, 21.039999999999999, 105.90000000000001, "Văn phòng Long Biên", null, null },
                    { 4, new DateTime(2026, 5, 23, 0, 0, 0, 0, DateTimeKind.Unspecified), "seed", null, null, "Hà Đông", false, false, 20.969999999999999, 105.76000000000001, "Trường THCS Hà Đông", null, null },
                    { 5, new DateTime(2026, 5, 23, 0, 0, 0, 0, DateTimeKind.Unspecified), "seed", null, null, "Hoàn Kiếm", false, true, 21.029, 105.852, "Khách sạn Hoàn Kiếm", null, null },
                    { 6, new DateTime(2026, 5, 23, 0, 0, 0, 0, DateTimeKind.Unspecified), "seed", null, null, "Ba Đình", false, false, 21.035, 105.81, "Quán phở Ba Đình", null, null },
                    { 7, new DateTime(2026, 5, 23, 0, 0, 0, 0, DateTimeKind.Unspecified), "seed", null, null, "Thanh Xuân", false, false, 20.995000000000001, 105.795, "Trung tâm Thanh Xuân", null, null },
                    { 8, new DateTime(2026, 5, 23, 0, 0, 0, 0, DateTimeKind.Unspecified), "seed", null, null, "Hai Bà Trưng", false, false, 21.004999999999999, 105.86499999999999, "Siêu thị Hai Bà Trưng", null, null }
                });

            migrationBuilder.InsertData(
                table: "RateCard",
                columns: new[] { "RateCardId", "BaseCost", "CreatedAt", "CreatedBy", "DeletedAt", "DeletedBy", "DropFee", "IsActive", "IsDeleted", "ToDistrict", "UpdatedAt", "UpdatedBy", "VehicleType", "VendorCode" },
                values: new object[,]
                {
                    { 1, 250000m, new DateTime(2026, 5, 23, 0, 0, 0, 0, DateTimeKind.Unspecified), "seed", null, null, 30000m, true, false, "Tây Hồ", null, null, "1.5T", "V1" },
                    { 2, 320000m, new DateTime(2026, 5, 23, 0, 0, 0, 0, DateTimeKind.Unspecified), "seed", null, null, 30000m, true, false, "Tây Hồ", null, null, "2.5T", "V1" },
                    { 3, 480000m, new DateTime(2026, 5, 23, 0, 0, 0, 0, DateTimeKind.Unspecified), "seed", null, null, 30000m, true, false, "Tây Hồ", null, null, "5T", "V1" },
                    { 4, 200000m, new DateTime(2026, 5, 23, 0, 0, 0, 0, DateTimeKind.Unspecified), "seed", null, null, 25000m, true, false, "Đống Đa", null, null, "1.5T", "V1" },
                    { 5, 270000m, new DateTime(2026, 5, 23, 0, 0, 0, 0, DateTimeKind.Unspecified), "seed", null, null, 25000m, true, false, "Đống Đa", null, null, "2.5T", "V1" },
                    { 6, 350000m, new DateTime(2026, 5, 23, 0, 0, 0, 0, DateTimeKind.Unspecified), "seed", null, null, 35000m, true, false, "Long Biên", null, null, "2.5T", "V1" },
                    { 7, 520000m, new DateTime(2026, 5, 23, 0, 0, 0, 0, DateTimeKind.Unspecified), "seed", null, null, 35000m, true, false, "Long Biên", null, null, "5T", "V1" },
                    { 8, 380000m, new DateTime(2026, 5, 23, 0, 0, 0, 0, DateTimeKind.Unspecified), "seed", null, null, 40000m, true, false, "Hà Đông", null, null, "2.5T", "V1" },
                    { 9, 550000m, new DateTime(2026, 5, 23, 0, 0, 0, 0, DateTimeKind.Unspecified), "seed", null, null, 40000m, true, false, "Hà Đông", null, null, "5T", "V1" },
                    { 10, 220000m, new DateTime(2026, 5, 23, 0, 0, 0, 0, DateTimeKind.Unspecified), "seed", null, null, 25000m, true, false, "Hoàn Kiếm", null, null, "1.5T", "V1" },
                    { 11, 230000m, new DateTime(2026, 5, 23, 0, 0, 0, 0, DateTimeKind.Unspecified), "seed", null, null, 25000m, true, false, "Ba Đình", null, null, "1.5T", "V1" },
                    { 12, 290000m, new DateTime(2026, 5, 23, 0, 0, 0, 0, DateTimeKind.Unspecified), "seed", null, null, 30000m, true, false, "Thanh Xuân", null, null, "2.5T", "V1" },
                    { 13, 450000m, new DateTime(2026, 5, 23, 0, 0, 0, 0, DateTimeKind.Unspecified), "seed", null, null, 30000m, true, false, "Thanh Xuân", null, null, "5T", "V1" },
                    { 14, 240000m, new DateTime(2026, 5, 23, 0, 0, 0, 0, DateTimeKind.Unspecified), "seed", null, null, 25000m, true, false, "Hai Bà Trưng", null, null, "1.5T", "V1" },
                    { 15, 460000m, new DateTime(2026, 5, 23, 0, 0, 0, 0, DateTimeKind.Unspecified), "seed", null, null, 30000m, true, false, "Tây Hồ", null, null, "5T", "V2" },
                    { 16, 500000m, new DateTime(2026, 5, 23, 0, 0, 0, 0, DateTimeKind.Unspecified), "seed", null, null, 35000m, true, false, "Long Biên", null, null, "5T", "V2" }
                });

            migrationBuilder.InsertData(
                table: "Vehicles",
                columns: new[] { "VehicleId", "CapacityKg", "CreatedAt", "CreatedBy", "DeletedAt", "DeletedBy", "IsActive", "IsDeleted", "Plate", "UpdatedAt", "UpdatedBy", "VehicleType", "VendorCode" },
                values: new object[,]
                {
                    { 1, 1500.0, new DateTime(2026, 5, 23, 0, 0, 0, 0, DateTimeKind.Unspecified), "seed", null, null, true, false, "29C-111.11", null, null, "1.5T", "V1" },
                    { 2, 2500.0, new DateTime(2026, 5, 23, 0, 0, 0, 0, DateTimeKind.Unspecified), "seed", null, null, true, false, "29C-222.22", null, null, "2.5T", "V1" },
                    { 3, 5000.0, new DateTime(2026, 5, 23, 0, 0, 0, 0, DateTimeKind.Unspecified), "seed", null, null, true, false, "29C-333.33", null, null, "5T", "V2" }
                });

            migrationBuilder.InsertData(
                table: "Vehicles",
                columns: new[] { "VehicleId", "CapacityKg", "CreatedAt", "CreatedBy", "DeletedAt", "DeletedBy", "IsActive", "IsDeleted", "Plate", "UpdatedAt", "UpdatedBy", "VehicleType", "VendorCode" },
                values: new object[] { 4, 2500.0, new DateTime(2026, 5, 23, 0, 0, 0, 0, DateTimeKind.Unspecified), "seed", null, null, false, false, "29C-444.44", null, null, "2.5T", "V2" });

            migrationBuilder.InsertData(
                table: "Orders",
                columns: new[] { "OrderId", "CreatedAt", "CreatedBy", "CustomerId", "DeletedAt", "DeletedBy", "DeliveryEnd", "DeliveryStart", "IsDeleted", "OrderDate", "Status", "UpdatedAt", "UpdatedBy", "WeightKg" },
                values: new object[,]
                {
                    { 1001, new DateTime(2026, 5, 23, 0, 0, 0, 0, DateTimeKind.Unspecified), "seed", 1, null, null, new TimeOnly(9, 0, 0), new TimeOnly(7, 0, 0), false, new DateOnly(2026, 5, 23), "NEW", null, null, 120.0 },
                    { 1002, new DateTime(2026, 5, 23, 0, 0, 0, 0, DateTimeKind.Unspecified), "seed", 2, null, null, new TimeOnly(10, 0, 0), new TimeOnly(7, 30, 0), false, new DateOnly(2026, 5, 23), "NEW", null, null, 80.0 },
                    { 1003, new DateTime(2026, 5, 23, 0, 0, 0, 0, DateTimeKind.Unspecified), "seed", 3, null, null, new TimeOnly(11, 0, 0), new TimeOnly(8, 0, 0), false, new DateOnly(2026, 5, 23), "NEW", null, null, 450.0 },
                    { 1004, new DateTime(2026, 5, 23, 0, 0, 0, 0, DateTimeKind.Unspecified), "seed", 4, null, null, new TimeOnly(10, 0, 0), new TimeOnly(7, 0, 0), false, new DateOnly(2026, 5, 23), "NEW", null, null, 300.0 },
                    { 1005, new DateTime(2026, 5, 23, 0, 0, 0, 0, DateTimeKind.Unspecified), "seed", 5, null, null, new TimeOnly(8, 30, 0), new TimeOnly(7, 0, 0), false, new DateOnly(2026, 5, 23), "NEW", null, null, 60.0 },
                    { 1006, new DateTime(2026, 5, 23, 0, 0, 0, 0, DateTimeKind.Unspecified), "seed", 6, null, null, new TimeOnly(10, 30, 0), new TimeOnly(7, 30, 0), false, new DateOnly(2026, 5, 23), "NEW", null, null, 200.0 },
                    { 1007, new DateTime(2026, 5, 23, 0, 0, 0, 0, DateTimeKind.Unspecified), "seed", 7, null, null, new TimeOnly(11, 0, 0), new TimeOnly(8, 0, 0), false, new DateOnly(2026, 5, 23), "NEW", null, null, 900.0 },
                    { 1008, new DateTime(2026, 5, 23, 0, 0, 0, 0, DateTimeKind.Unspecified), "seed", 8, null, null, new TimeOnly(9, 30, 0), new TimeOnly(7, 0, 0), false, new DateOnly(2026, 5, 23), "NEW", null, null, 150.0 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Orders_CustomerId",
                table: "Orders",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_OrderDate_Status",
                table: "Orders",
                columns: new[] { "OrderDate", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_RateCard_VendorCode_ToDistrict_VehicleType",
                table: "RateCard",
                columns: new[] { "VendorCode", "ToDistrict", "VehicleType" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TripOrders_OrderId",
                table: "TripOrders",
                column: "OrderId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TripOrders_TripId_OrderId",
                table: "TripOrders",
                columns: new[] { "TripId", "OrderId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Trips_PlanCode",
                table: "Trips",
                column: "PlanCode");

            migrationBuilder.CreateIndex(
                name: "IX_Trips_VehicleId",
                table: "Trips",
                column: "VehicleId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RateCard");

            migrationBuilder.DropTable(
                name: "TripOrders");

            migrationBuilder.DropTable(
                name: "Orders");

            migrationBuilder.DropTable(
                name: "Trips");

            migrationBuilder.DropTable(
                name: "Customers");

            migrationBuilder.DropTable(
                name: "Vehicles");
        }
    }
}
