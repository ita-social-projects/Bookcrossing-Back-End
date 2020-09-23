using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BookCrossingBackEnd.Migrations
{
    public partial class AddLocationHome : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "LocationHome",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Book",
                keyColumn: "id",
                keyValue: 1,
                columns: new[] { "DateAdded", "State" },
                values: new object[] { new DateTime(2020, 9, 19, 16, 31, 42, 880, DateTimeKind.Local).AddTicks(7662), "Available" });

            migrationBuilder.UpdateData(
                table: "LocationHome",
                keyColumn: "Id",
                keyValue: 1,
                column: "UserId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "User",
                keyColumn: "id",
                keyValue: 1,
                columns: new[] { "password", "role_id" },
                values: new object[] { "test", 1 });

            migrationBuilder.UpdateData(
                table: "User",
                keyColumn: "id",
                keyValue: 2,
                columns: new[] { "password", "role_id" },
                values: new object[] { "admin", 2 });

            migrationBuilder.UpdateData(
                table: "User",
                keyColumn: "id",
                keyValue: 3,
                column: "role_id",
                value: 2);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserId",
                table: "LocationHome");

            migrationBuilder.UpdateData(
                table: "Book",
                keyColumn: "id",
                keyValue: 1,
                columns: new[] { "DateAdded", "State" },
                values: new object[] { new DateTime(2020, 9, 19, 14, 11, 8, 709, DateTimeKind.Local).AddTicks(5807), "Available" });

            migrationBuilder.UpdateData(
                table: "User",
                keyColumn: "id",
                keyValue: 1,
                columns: new[] { "password", "role_id" },
                values: new object[] { "AQAAAAEAACcQAAAAEF4qkrO2H0/sZvpx2lCnaFSQhEJZ5/vsMx4V3ZD3x8529ymU0VjTytn3HA94R/RSmw==", 1 });

            migrationBuilder.UpdateData(
                table: "User",
                keyColumn: "id",
                keyValue: 2,
                columns: new[] { "password", "role_id" },
                values: new object[] { "AQAAAAEAACcQAAAAEF4qkrO2H0/sZvpx2lCnaFSQhEJZ5/vsMx4V3ZD3x8529ymU0VjTytn3HA94R/RSmw==", 2 });

            migrationBuilder.UpdateData(
                table: "User",
                keyColumn: "id",
                keyValue: 3,
                column: "role_id",
                value: 2);
        }
    }
}
