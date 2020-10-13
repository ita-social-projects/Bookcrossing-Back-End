using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BookCrossingBackEnd.Migrations
{
    public partial class NewLanguageDataInSeeder : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Street",
                table: "LocationHome",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "City",
                table: "LocationHome",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ISBN",
                table: "Book",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(17)",
                oldNullable: true);

            migrationBuilder.UpdateData(
                table: "Book",
                keyColumn: "id",
                keyValue: 1,
                columns: new[] { "DateAdded", "State" },
                values: new object[] { new DateTime(2020, 10, 12, 20, 33, 51, 253, DateTimeKind.Local).AddTicks(5844), "Available" });

            migrationBuilder.UpdateData(
                table: "Language",
                keyColumn: "id",
                keyValue: 1,
                column: "name",
                value: "English");

            migrationBuilder.InsertData(
                table: "Language",
                columns: new[] { "id", "name" },
                values: new object[,]
                {
                    { 2, "Українська" },
                    { 3, "Русский" },
                    { 4, "Italiano" },
                    { 5, "Deutsche" },
                    { 6, "Español" },
                    { 7, "Polskie" },
                    { 8, "Беларуская" },
                    { 9, "Français" },
                    { 10, "Português" }
                });

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

            migrationBuilder.UpdateData(
                table: "User",
                keyColumn: "id",
                keyValue: 3,
                column: "role_id",
                value: 2);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Language",
                keyColumn: "id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Language",
                keyColumn: "id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Language",
                keyColumn: "id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Language",
                keyColumn: "id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Language",
                keyColumn: "id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Language",
                keyColumn: "id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Language",
                keyColumn: "id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Language",
                keyColumn: "id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Language",
                keyColumn: "id",
                keyValue: 10);

            migrationBuilder.AlterColumn<string>(
                name: "Street",
                table: "LocationHome",
                type: "nvarchar(255)",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "City",
                table: "LocationHome",
                type: "nvarchar(255)",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ISBN",
                table: "Book",
                type: "nvarchar(17)",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.UpdateData(
                table: "Book",
                keyColumn: "id",
                keyValue: 1,
                columns: new[] { "DateAdded", "State" },
                values: new object[] { new DateTime(2020, 9, 28, 16, 25, 16, 370, DateTimeKind.Local).AddTicks(8555), "Available" });

            migrationBuilder.UpdateData(
                table: "Language",
                keyColumn: "id",
                keyValue: 1,
                column: "name",
                value: "Unknown");

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

            migrationBuilder.UpdateData(
                table: "User",
                keyColumn: "id",
                keyValue: 3,
                column: "role_id",
                value: 2);
        }
    }
}
