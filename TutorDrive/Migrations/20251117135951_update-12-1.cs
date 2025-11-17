using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TutorDrive.Migrations
{
    /// <inheritdoc />
    public partial class update121 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "DistanceKm",
                table: "StudentProfiles",
                type: "decimal(6,2)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DurationMinutes",
                table: "StudentProfiles",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "CourseId",
                table: "Feedbacks",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Feedbacks_CourseId",
                table: "Feedbacks",
                column: "CourseId");

            migrationBuilder.AddForeignKey(
                name: "FK_Feedbacks_Courses_CourseId",
                table: "Feedbacks",
                column: "CourseId",
                principalTable: "Courses",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Feedbacks_Courses_CourseId",
                table: "Feedbacks");

            migrationBuilder.DropIndex(
                name: "IX_Feedbacks_CourseId",
                table: "Feedbacks");

            migrationBuilder.DropColumn(
                name: "DistanceKm",
                table: "StudentProfiles");

            migrationBuilder.DropColumn(
                name: "DurationMinutes",
                table: "StudentProfiles");

            migrationBuilder.DropColumn(
                name: "CourseId",
                table: "Feedbacks");
        }
    }
}
