using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TutorDrive.Migrations
{
    /// <inheritdoc />
    public partial class InstructorProfile : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Feedbacks_Staffs_StaffId",
                table: "Feedbacks");

            migrationBuilder.DropForeignKey(
                name: "FK_LearningProgresses_Staffs_StaffId",
                table: "LearningProgresses");

            migrationBuilder.DropForeignKey(
                name: "FK_Schedules_Staffs_StaffId",
                table: "Schedules");

            migrationBuilder.RenameColumn(
                name: "StaffId",
                table: "Schedules",
                newName: "InstructorProfileId");

            migrationBuilder.RenameIndex(
                name: "IX_Schedules_StaffId",
                table: "Schedules",
                newName: "IX_Schedules_InstructorProfileId");

            migrationBuilder.RenameColumn(
                name: "StaffId",
                table: "LearningProgresses",
                newName: "InstructorProfileId");

            migrationBuilder.RenameIndex(
                name: "IX_LearningProgresses_StaffId",
                table: "LearningProgresses",
                newName: "IX_LearningProgresses_InstructorProfileId");

            migrationBuilder.RenameColumn(
                name: "StaffId",
                table: "Feedbacks",
                newName: "InstructorProfileId");

            migrationBuilder.RenameIndex(
                name: "IX_Feedbacks_StaffId",
                table: "Feedbacks",
                newName: "IX_Feedbacks_InstructorProfileId");

            migrationBuilder.AddForeignKey(
                name: "FK_Feedbacks_Staffs_InstructorProfileId",
                table: "Feedbacks",
                column: "InstructorProfileId",
                principalTable: "Staffs",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_LearningProgresses_Staffs_InstructorProfileId",
                table: "LearningProgresses",
                column: "InstructorProfileId",
                principalTable: "Staffs",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Schedules_Staffs_InstructorProfileId",
                table: "Schedules",
                column: "InstructorProfileId",
                principalTable: "Staffs",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Feedbacks_Staffs_InstructorProfileId",
                table: "Feedbacks");

            migrationBuilder.DropForeignKey(
                name: "FK_LearningProgresses_Staffs_InstructorProfileId",
                table: "LearningProgresses");

            migrationBuilder.DropForeignKey(
                name: "FK_Schedules_Staffs_InstructorProfileId",
                table: "Schedules");

            migrationBuilder.RenameColumn(
                name: "InstructorProfileId",
                table: "Schedules",
                newName: "StaffId");

            migrationBuilder.RenameIndex(
                name: "IX_Schedules_InstructorProfileId",
                table: "Schedules",
                newName: "IX_Schedules_StaffId");

            migrationBuilder.RenameColumn(
                name: "InstructorProfileId",
                table: "LearningProgresses",
                newName: "StaffId");

            migrationBuilder.RenameIndex(
                name: "IX_LearningProgresses_InstructorProfileId",
                table: "LearningProgresses",
                newName: "IX_LearningProgresses_StaffId");

            migrationBuilder.RenameColumn(
                name: "InstructorProfileId",
                table: "Feedbacks",
                newName: "StaffId");

            migrationBuilder.RenameIndex(
                name: "IX_Feedbacks_InstructorProfileId",
                table: "Feedbacks",
                newName: "IX_Feedbacks_StaffId");

            migrationBuilder.AddForeignKey(
                name: "FK_Feedbacks_Staffs_StaffId",
                table: "Feedbacks",
                column: "StaffId",
                principalTable: "Staffs",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_LearningProgresses_Staffs_StaffId",
                table: "LearningProgresses",
                column: "StaffId",
                principalTable: "Staffs",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Schedules_Staffs_StaffId",
                table: "Schedules",
                column: "StaffId",
                principalTable: "Staffs",
                principalColumn: "Id");
        }
    }
}
