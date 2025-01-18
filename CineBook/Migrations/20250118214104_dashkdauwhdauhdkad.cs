using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CineBook.Migrations
{
    /// <inheritdoc />
    public partial class dashkdauwhdauhdkad : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SupportTicketTicketId",
                table: "Messages",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_SupportTicketTicketId",
                table: "Messages",
                column: "SupportTicketTicketId");

            migrationBuilder.AddForeignKey(
                name: "FK_Messages_SupportTickets_SupportTicketTicketId",
                table: "Messages",
                column: "SupportTicketTicketId",
                principalTable: "SupportTickets",
                principalColumn: "TicketId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Messages_SupportTickets_SupportTicketTicketId",
                table: "Messages");

            migrationBuilder.DropIndex(
                name: "IX_Messages_SupportTicketTicketId",
                table: "Messages");

            migrationBuilder.DropColumn(
                name: "SupportTicketTicketId",
                table: "Messages");
        }
    }
}
