using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BookCrossingBackEnd.Migrations
{
    public partial class AddColumnNameUkToIssueTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "SuggestionMessage",
                keyColumn: "id",
                keyValue: 1);

            migrationBuilder.AddColumn<string>(
                name: "nameUk",
                table: "Issue",
                maxLength: 30,
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "Book",
                keyColumn: "id",
                keyValue: 1,
                columns: new[] { "DateAdded", "State" },
                values: new object[] { new DateTime(2020, 9, 28, 16, 25, 16, 370, DateTimeKind.Local).AddTicks(8555), "Available" });

            migrationBuilder.UpdateData(
                table: "Issue",
                keyColumn: "id",
                keyValue: 1,
                column: "nameUk",
                value: "Загальне");

            migrationBuilder.UpdateData(
                table: "Issue",
                keyColumn: "id",
                keyValue: 2,
                column: "nameUk",
                value: "Пропозиція щодо вдосконалення");

            migrationBuilder.UpdateData(
                table: "Issue",
                keyColumn: "id",
                keyValue: 3,
                column: "nameUk",
                value: "Потрібна підтримка");

            migrationBuilder.UpdateData(
                table: "Issue",
                keyColumn: "id",
                keyValue: 4,
                column: "nameUk",
                value: "Знайдено помилку");

            migrationBuilder.UpdateData(
                table: "Issue",
                keyColumn: "id",
                keyValue: 5,
                column: "nameUk",
                value: "Обмін книгами");

            migrationBuilder.UpdateData(
                table: "Issue",
                keyColumn: "id",
                keyValue: 6,
                column: "nameUk",
                value: "Інше");

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
            migrationBuilder.DropColumn(
                name: "nameUk",
                table: "Issue");

            migrationBuilder.UpdateData(
                table: "Book",
                keyColumn: "id",
                keyValue: 1,
                columns: new[] { "DateAdded", "State" },
                values: new object[] { new DateTime(2020, 9, 23, 22, 30, 2, 575, DateTimeKind.Local).AddTicks(4505), "Available" });

            migrationBuilder.InsertData(
                table: "SuggestionMessage",
                columns: new[] { "id", "State", "summary", "text", "user_id" },
                values: new object[] { 1, "Unread", "fix problem", "There is problem with translation", 1 });

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
