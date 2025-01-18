using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CineBook.Migrations
{
    /// <inheritdoc />
    public partial class ashdaw : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SupportTickets_Chats_ChatId",
                table: "SupportTickets");

            migrationBuilder.DropIndex(
                name: "IX_SupportTickets_ChatId",
                table: "SupportTickets");

            migrationBuilder.DropColumn(
                name: "ChatId",
                table: "SupportTickets");

            migrationBuilder.AddColumn<string>(
                name: "TicketId",
                table: "Messages",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TicketId",
                table: "Messages");

            migrationBuilder.AddColumn<string>(
                name: "ChatId",
                table: "SupportTickets",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_SupportTickets_ChatId",
                table: "SupportTickets",
                column: "ChatId");

            migrationBuilder.AddForeignKey(
                name: "FK_SupportTickets_Chats_ChatId",
                table: "SupportTickets",
                column: "ChatId",
                principalTable: "Chats",
                principalColumn: "ChatId");
        }
    }
}
