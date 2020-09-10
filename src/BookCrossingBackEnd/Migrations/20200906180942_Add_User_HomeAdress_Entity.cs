using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BookCrossingBackEnd.Migrations
{
    public partial class Add_User_HomeAdress_Entity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HomeAdress",
                table: "Location");

            migrationBuilder.AddColumn<int>(
                name: "UserHomeAdressId",
                table: "User",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "UserHomeAdress",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LocationId = table.Column<int>(nullable: false),
                    HomeAdress = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserHomeAdress", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserHomeAdress_Location_LocationId",
                        column: x => x.LocationId,
                        principalTable: "Location",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

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

            migrationBuilder.CreateIndex(
                name: "IX_User_UserHomeAdressId",
                table: "User",
                column: "UserHomeAdressId");

            migrationBuilder.CreateIndex(
                name: "IX_UserHomeAdress_LocationId",
                table: "UserHomeAdress",
                column: "LocationId");

            migrationBuilder.AddForeignKey(
                name: "FK_User_UserHomeAdress_UserHomeAdressId",
                table: "User",
                column: "UserHomeAdressId",
                principalTable: "UserHomeAdress",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_User_UserHomeAdress_UserHomeAdressId",
                table: "User");

            migrationBuilder.DropTable(
                name: "UserHomeAdress");

            migrationBuilder.DropIndex(
                name: "IX_User_UserHomeAdressId",
                table: "User");

            migrationBuilder.DropColumn(
                name: "UserHomeAdressId",
                table: "User");

            migrationBuilder.AddColumn<string>(
                name: "HomeAdress",
                table: "Location",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Book",
                keyColumn: "id",
                keyValue: 1,
                columns: new[] { "DateAdded", "State" },
                values: new object[] { new DateTime(2020, 9, 5, 23, 7, 28, 127, DateTimeKind.Local).AddTicks(7668), "Available" });

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
