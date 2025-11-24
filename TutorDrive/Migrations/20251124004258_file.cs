using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TutorDrive.Migrations
{
    /// <inheritdoc />
    public partial class fileImport : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Score",
                table: "ExamResults");

            migrationBuilder.AddColumn<string>(
                name: "ExamCode",
                table: "Exams",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<int>(
                name: "Status",
                table: "ExamResults",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<string>(
                name: "ExamCode",
                table: "ExamResults",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<float>(
                name: "RoadTestScore",
                table: "ExamResults",
                type: "real",
                nullable: true);

            migrationBuilder.AddColumn<float>(
                name: "SimulationScore",
                table: "ExamResults",
                type: "real",
                nullable: true);

            migrationBuilder.AddColumn<float>(
                name: "TheoryScore",
                table: "ExamResults",
                type: "real",
                nullable: true);

            migrationBuilder.AddColumn<float>(
                name: "TrackScore",
                table: "ExamResults",
                type: "real",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExamCode",
                table: "Exams");

            migrationBuilder.DropColumn(
                name: "ExamCode",
                table: "ExamResults");

            migrationBuilder.DropColumn(
                name: "RoadTestScore",
                table: "ExamResults");

            migrationBuilder.DropColumn(
                name: "SimulationScore",
                table: "ExamResults");

            migrationBuilder.DropColumn(
                name: "TheoryScore",
                table: "ExamResults");

            migrationBuilder.DropColumn(
                name: "TrackScore",
                table: "ExamResults");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "ExamResults",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<decimal>(
                name: "Score",
                table: "ExamResults",
                type: "decimal(18,2)",
                nullable: true);
        }
    }
}
