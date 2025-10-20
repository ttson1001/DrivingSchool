using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TutorDrive.Migrations
{
    /// <inheritdoc />
    public partial class updat5 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Avalar",
                table: "StudentProfiles");

            migrationBuilder.AddColumn<string>(
                name: "Avalar",
                table: "Accounts",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Avalar",
                table: "Accounts");

            migrationBuilder.AddColumn<string>(
                name: "Avalar",
                table: "StudentProfiles",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
