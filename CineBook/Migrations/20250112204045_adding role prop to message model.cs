using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CineBook.Migrations
{
    /// <inheritdoc />
    public partial class addingroleproptomessagemodel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "AgentId",
                table: "Messages",
                newName: "Role");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Role",
                table: "Messages",
                newName: "AgentId");
        }
    }
}
