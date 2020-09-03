using Microsoft.EntityFrameworkCore.Migrations;

namespace BookCrossingBackEnd.Migrations
{
    public partial class AddBookRating : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.CreateTable(
                name: "BookRating",
                columns: table => new
                {
                    book_id = table.Column<int>(nullable: false),
                    user_id = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BookRating", x => new { x.book_id, x.user_id });
                    table.ForeignKey(
                        name: "FK_BookRating_Book_book_id",
                        column: x => x.book_id,
                        principalTable: "Book",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BookRating_User_user_id",
                        column: x => x.user_id,
                        principalTable: "User",
                        principalColumn: "id",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BookRating_user_id",
                table: "BookRating",
                column: "user_id");

            migrationBuilder.AddColumn<double>(
                name: "Rating",
                table: "BookRating",
                nullable: false,
                defaultValue: 0.0);

        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BookRating");
        }
    }
}
