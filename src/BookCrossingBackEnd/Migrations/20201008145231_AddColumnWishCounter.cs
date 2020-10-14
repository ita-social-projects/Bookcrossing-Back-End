using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BookCrossingBackEnd.Migrations
{
    public partial class AddColumnWishCounter : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.AddColumn<int>(
                name: "WishCount",
                table: "Book",
                nullable: false,
                defaultValue: 0);

        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "WishCount",
                table: "Book");
        }
    }
}
