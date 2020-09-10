using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BookCrossingBackEnd.Migrations
{
    public partial class AddUserHomeAdress_ToOthersEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Book",
                keyColumn: "id",
                keyValue: 1,
                columns: new[] { "DateAdded", "State" },
                values: new object[] { new DateTime(2020, 9, 10, 16, 12, 41, 251, DateTimeKind.Local).AddTicks(6808), "Available" });

            migrationBuilder.UpdateData(
                table: "User",
                keyColumn: "id",
                keyValue: 1,
                column: "role_id",
                value: 1);

            migrationBuilder.UpdateData(
                table: "User",
                keyColumn: "id",
                keyValue: 2,
                column: "role_id",
                value: 2);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Book",
                keyColumn: "id",
                keyValue: 1,
                columns: new[] { "DateAdded", "State" },
                values: new object[] { new DateTime(2020, 9, 6, 21, 9, 41, 854, DateTimeKind.Local).AddTicks(5965), "Available" });

            migrationBuilder.UpdateData(
                table: "User",
                keyColumn: "id",
                keyValue: 1,
                column: "role_id",
                value: 1);

            migrationBuilder.UpdateData(
                table: "User",
                keyColumn: "id",
                keyValue: 2,
                column: "role_id",
                value: 2);
        }
    }
}
