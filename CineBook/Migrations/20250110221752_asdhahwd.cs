﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CineBook.Migrations
{
    /// <inheritdoc />
    public partial class asdhahwd : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "VerificationToken",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "VerificationToken",
                table: "AspNetUsers");
        }
    }
}
