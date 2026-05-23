using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MiniTms.Data.Migrations
{
    /// <inheritdoc />
    public partial class FixVehicle4InactiveSeed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Vehicles",
                keyColumn: "VehicleId",
                keyValue: 4,
                column: "IsActive",
                value: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Vehicles",
                keyColumn: "VehicleId",
                keyValue: 4,
                column: "IsActive",
                value: true);
        }
    }
}
