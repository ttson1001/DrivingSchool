using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TutorDrive.Migrations
{
    /// <inheritdoc />
    public partial class FixLearningProgressIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
                migrationBuilder.DropIndex(
            name: "IX_LearningProgresses_StudentProfileId",
            table: "LearningProgresses");

            migrationBuilder.CreateIndex(
                name: "IX_LearningProgresses_StudentProfileId",
                table: "LearningProgresses",
                column: "StudentProfileId");


        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
