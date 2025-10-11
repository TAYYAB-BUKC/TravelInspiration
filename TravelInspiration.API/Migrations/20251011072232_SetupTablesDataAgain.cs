using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TravelInspiration.API.Migrations
{
    /// <inheritdoc />
    public partial class SetupTablesDataAgain : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Itineraries",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedOn", "UserId" },
                values: new object[] { new DateTime(2025, 10, 11, 7, 16, 39, 963, DateTimeKind.Utc), "UserId" });

            migrationBuilder.UpdateData(
                table: "Itineraries",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedOn", "UserId" },
                values: new object[] { new DateTime(2025, 10, 11, 7, 16, 39, 963, DateTimeKind.Utc), "UserId" });

            migrationBuilder.UpdateData(
                table: "Stops",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedOn",
                value: new DateTime(2025, 10, 11, 7, 16, 39, 963, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Stops",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedOn",
                value: new DateTime(2025, 10, 11, 7, 16, 39, 963, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Stops",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedOn",
                value: new DateTime(2025, 10, 11, 7, 16, 39, 963, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Stops",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedOn",
                value: new DateTime(2025, 10, 11, 7, 16, 39, 963, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Stops",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedOn",
                value: new DateTime(2025, 10, 11, 7, 16, 39, 963, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Stops",
                keyColumn: "Id",
                keyValue: 6,
                column: "CreatedOn",
                value: new DateTime(2025, 10, 11, 7, 16, 39, 963, DateTimeKind.Utc));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Itineraries",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedOn", "UserId" },
                values: new object[] { new DateTime(2025, 10, 11, 7, 16, 39, 961, DateTimeKind.Utc).AddTicks(2977), "KevinsUserId" });

            migrationBuilder.UpdateData(
                table: "Itineraries",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedOn", "UserId" },
                values: new object[] { new DateTime(2025, 10, 11, 7, 16, 39, 961, DateTimeKind.Utc).AddTicks(3244), "KevinsUserId" });

            migrationBuilder.UpdateData(
                table: "Stops",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedOn",
                value: new DateTime(2025, 10, 11, 7, 16, 39, 963, DateTimeKind.Utc).AddTicks(3332));

            migrationBuilder.UpdateData(
                table: "Stops",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedOn",
                value: new DateTime(2025, 10, 11, 7, 16, 39, 963, DateTimeKind.Utc).AddTicks(4406));

            migrationBuilder.UpdateData(
                table: "Stops",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedOn",
                value: new DateTime(2025, 10, 11, 7, 16, 39, 963, DateTimeKind.Utc).AddTicks(4424));

            migrationBuilder.UpdateData(
                table: "Stops",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedOn",
                value: new DateTime(2025, 10, 11, 7, 16, 39, 963, DateTimeKind.Utc).AddTicks(4429));

            migrationBuilder.UpdateData(
                table: "Stops",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedOn",
                value: new DateTime(2025, 10, 11, 7, 16, 39, 963, DateTimeKind.Utc).AddTicks(4432));

            migrationBuilder.UpdateData(
                table: "Stops",
                keyColumn: "Id",
                keyValue: 6,
                column: "CreatedOn",
                value: new DateTime(2025, 10, 11, 7, 16, 39, 963, DateTimeKind.Utc).AddTicks(4437));
        }
    }
}
