using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TutorDrive.Migrations
{
    /// <inheritdoc />
    public partial class okay : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Date",
                table: "Exams",
                newName: "ExamDate");

            migrationBuilder.AlterColumn<int>(
                name: "Type",
                table: "Exams",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateTable(
                name: "RegistrationExamExams",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RegistrationExamId = table.Column<long>(type: "bigint", nullable: false),
                    ExamId = table.Column<long>(type: "bigint", nullable: false)
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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RegistrationExamExams");

            migrationBuilder.RenameColumn(
                name: "ExamDate",
                table: "Exams",
                newName: "Date");

            migrationBuilder.AlterColumn<string>(
                name: "Type",
                table: "Exams",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");
        }
    }
}
