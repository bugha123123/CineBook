using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CineBook.Migrations
{
    /// <inheritdoc />
    public partial class addingClosedAtproptoSupportTicketModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AgentJoined",
                table: "SupportTickets");

            migrationBuilder.AddColumn<DateTime>(
                name: "ClosedAt",
                table: "SupportTickets",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ClosedAt",
                table: "SupportTickets");

            migrationBuilder.AddColumn<bool>(
                name: "AgentJoined",
                table: "SupportTickets",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
