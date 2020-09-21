using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BookCrossingBackEnd.Migrations
{
    public partial class AddUserIdToNotifications : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.AddColumn<int>(
                name: "ReceiverUserId",
                table: "Notification",
                nullable: true); 

            migrationBuilder.AddForeignKey(
                name: "FK_Notification_Receiver_user_id",
                table: "Notification",
                column: "ReceiverUserId",
                principalTable: "User",
                principalColumn: "id",
                onDelete: ReferentialAction.NoAction);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Notification_Receiver_user_id",
                table: "Notification");

            migrationBuilder.DropColumn(
                name: "ReceiverUserId",
                table: "Notification");
        }
    }
}
