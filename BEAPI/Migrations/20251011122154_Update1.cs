using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BEAPI.Migrations
{
    /// <inheritdoc />
    public partial class Update1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Wards_Provinces_ProvinceId",
                table: "Wards");

            migrationBuilder.DropIndex(
                name: "IX_Wards_ProvinceId",
                table: "Wards");

            migrationBuilder.DropIndex(
                name: "IX_StudentProfiles_AccountId",
                table: "StudentProfiles");

            migrationBuilder.DropColumn(
                name: "ProvinceId",
                table: "Wards");

            migrationBuilder.AddColumn<string>(
                name: "Code",
                table: "Provinces",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Type",
                table: "Provinces",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_StudentProfiles_AccountId",
                table: "StudentProfiles",
                column: "AccountId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_StudentProfiles_AccountId",
                table: "StudentProfiles");

            migrationBuilder.DropColumn(
                name: "Code",
                table: "Provinces");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "Provinces");

            migrationBuilder.AddColumn<long>(
                name: "ProvinceId",
                table: "Wards",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Wards_ProvinceId",
                table: "Wards",
                column: "ProvinceId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentProfiles_AccountId",
                table: "StudentProfiles",
                column: "AccountId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Wards_Provinces_ProvinceId",
                table: "Wards",
                column: "ProvinceId",
                principalTable: "Provinces",
                principalColumn: "Id");
        }
    }
}
