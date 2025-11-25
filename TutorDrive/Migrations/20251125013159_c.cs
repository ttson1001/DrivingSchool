using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TutorDrive.Migrations
{
    /// <inheritdoc />
    public partial class c : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RegistrationExamExams");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "Exams");

            migrationBuilder.AddColumn<long>(
                name: "ExamId",
                table: "RegistrationExams",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "RoadTest",
                table: "Exams",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Simulation",
                table: "Exams",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Theory",
                table: "Exams",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Track",
                table: "Exams",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_RegistrationExams_ExamId",
                table: "RegistrationExams",
                column: "ExamId");

            migrationBuilder.AddForeignKey(
                name: "FK_RegistrationExams_Exams_ExamId",
                table: "RegistrationExams",
                column: "ExamId",
                principalTable: "Exams",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RegistrationExams_Exams_ExamId",
                table: "RegistrationExams");

            migrationBuilder.DropIndex(
                name: "IX_RegistrationExams_ExamId",
                table: "RegistrationExams");

            migrationBuilder.DropColumn(
                name: "ExamId",
                table: "RegistrationExams");

            migrationBuilder.DropColumn(
                name: "RoadTest",
                table: "Exams");

            migrationBuilder.DropColumn(
                name: "Simulation",
                table: "Exams");

            migrationBuilder.DropColumn(
                name: "Theory",
                table: "Exams");

            migrationBuilder.DropColumn(
                name: "Track",
                table: "Exams");

            migrationBuilder.AddColumn<int>(
                name: "Type",
                table: "Exams",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "RegistrationExamExams",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ExamId = table.Column<long>(type: "bigint", nullable: false),
                    RegistrationExamId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RegistrationExamExams", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RegistrationExamExams_Exams_ExamId",
                        column: x => x.ExamId,
                        principalTable: "Exams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RegistrationExamExams_RegistrationExams_RegistrationExamId",
                        column: x => x.RegistrationExamId,
                        principalTable: "RegistrationExams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RegistrationExamExams_ExamId",
                table: "RegistrationExamExams",
                column: "ExamId");

            migrationBuilder.CreateIndex(
                name: "IX_RegistrationExamExams_RegistrationExamId",
                table: "RegistrationExamExams",
                column: "RegistrationExamId");
        }
    }
}
