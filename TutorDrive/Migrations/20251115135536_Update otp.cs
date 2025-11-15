using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TutorDrive.Migrations
{
    /// <inheritdoc />
    public partial class Updateotp : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ResetOtp",
                table: "Accounts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ResetOtpExpiry",
                table: "Accounts",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ResetOtp",
                table: "Accounts");

            migrationBuilder.DropColumn(
                name: "ResetOtpExpiry",
                table: "Accounts");
        }
    }
}
