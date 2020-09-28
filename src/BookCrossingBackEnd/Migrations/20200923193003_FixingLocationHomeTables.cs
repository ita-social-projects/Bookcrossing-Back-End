using Microsoft.EntityFrameworkCore.Migrations;

namespace BookCrossingBackEnd.Migrations
{
    public partial class FixingLocationHomeTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "home_location_id",
                table: "User",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "LocationHome",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    City = table.Column<string>(nullable: true),
                    Street = table.Column<string>(nullable: true),
                    IsActive = table.Column<bool>(nullable: false),
                    Latitude = table.Column<double>(nullable: false),
                    Longitude = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LocationHome", x => x.Id);
                });


            migrationBuilder.AddForeignKey(
                name: "FK_User_LocationHome_home_location_id",
                table: "User",
                column: "home_location_id",
                principalTable: "LocationHome",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_User_LocationHome_home_location_id",
                table: "User");

            migrationBuilder.DropTable(
                name: "LocationHome");
        }
    }
}