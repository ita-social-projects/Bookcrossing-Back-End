using Microsoft.EntityFrameworkCore.Migrations;

namespace BookCrossingBackEnd.Migrations
{
    public partial class AddedSettingsTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Setting",
                columns: table => new
                {
                    Namespace = table.Column<string>(nullable: false, defaultValue: "Global"),
                    Key = table.Column<string>(nullable: false),
                    Value = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Setting", x => new { x.Namespace, x.Key });
                });

            migrationBuilder.InsertData(
                table: "Setting",
                columns: new[] { "Namespace", "Key", "Description", "Value" },
                values: new object[] { "Timespans", "RequestAutoCancelTimespan", null, null });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Setting");
        }
    }
}
