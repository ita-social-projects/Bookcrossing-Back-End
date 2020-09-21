using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BookCrossingBackEnd.Migrations
{
    public partial class AddSuggestionMessageTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SuggestionMessage",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    summary = table.Column<string>(maxLength: 150, nullable: true),
                    text = table.Column<string>(maxLength: 500, nullable: false),
                    State = table.Column<string>(maxLength: 50, nullable: false, defaultValue: "Unread"),
                    user_id = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SuggestionMessage", x => x.id);
                    table.ForeignKey(
                        name: "FK_SuggestionMessage_User_user_id",
                        column: x => x.user_id,
                        principalTable: "User",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "Book",
                keyColumn: "id",
                keyValue: 1,
                columns: new[] { "DateAdded", "State" },
                values: new object[] { new DateTime(2020, 9, 20, 20, 26, 40, 856, DateTimeKind.Local).AddTicks(2859), "Available" });

            migrationBuilder.InsertData(
                table: "SuggestionMessage",
                columns: new[] { "id", "summary", "text", "user_id" },
                values: new object[] { 1, "fix problem", "There is problem with translation", 1 });

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

            migrationBuilder.CreateIndex(
                name: "IX_SuggestionMessage_user_id",
                table: "SuggestionMessage",
                column: "user_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SuggestionMessage");

            migrationBuilder.CreateTable(
                name: "Message",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    State = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, defaultValue: "Unread"),
                    summary = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    text = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    user_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Message", x => x.id);
                    table.ForeignKey(
                        name: "FK_Message_User_user_id",
                        column: x => x.user_id,
                        principalTable: "User",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "Book",
                keyColumn: "id",
                keyValue: 1,
                columns: new[] { "DateAdded", "State" },
                values: new object[] { new DateTime(2020, 9, 16, 18, 55, 40, 513, DateTimeKind.Local).AddTicks(5344), "Available" });

            migrationBuilder.InsertData(
                table: "Message",
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

            migrationBuilder.CreateIndex(
                name: "IX_Message_user_id",
                table: "Message",
                column: "user_id");
        }
    }
}
