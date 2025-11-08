using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TutorDrive.Migrations
{
    /// <inheritdoc />
    public partial class instructor : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Feedbacks_Staffs_InstructorProfileId",
                table: "Feedbacks");

            migrationBuilder.DropForeignKey(
                name: "FK_LearningProgresses_Staffs_InstructorProfileId",
                table: "LearningProgresses");

            migrationBuilder.DropForeignKey(
                name: "FK_Staffs_Accounts_AccountId",
                table: "Staffs");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Staffs",
                table: "Staffs");

            migrationBuilder.RenameTable(
                name: "Staffs",
                newName: "InstructorProfiles");

            migrationBuilder.RenameIndex(
                name: "IX_Staffs_AccountId",
                table: "InstructorProfiles",
                newName: "IX_InstructorProfiles_AccountId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_InstructorProfiles",
                table: "InstructorProfiles",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Feedbacks_InstructorProfiles_InstructorProfileId",
                table: "Feedbacks",
                column: "InstructorProfileId",
                principalTable: "InstructorProfiles",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_InstructorProfiles_Accounts_AccountId",
                table: "InstructorProfiles",
                column: "AccountId",
                principalTable: "Accounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_LearningProgresses_InstructorProfiles_InstructorProfileId",
                table: "LearningProgresses",
                column: "InstructorProfileId",
                principalTable: "InstructorProfiles",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Feedbacks_InstructorProfiles_InstructorProfileId",
                table: "Feedbacks");

            migrationBuilder.DropForeignKey(
                name: "FK_InstructorProfiles_Accounts_AccountId",
                table: "InstructorProfiles");

            migrationBuilder.DropForeignKey(
                name: "FK_LearningProgresses_InstructorProfiles_InstructorProfileId",
                table: "LearningProgresses");

            migrationBuilder.DropPrimaryKey(
                name: "PK_InstructorProfiles",
                table: "InstructorProfiles");

            migrationBuilder.RenameTable(
                name: "InstructorProfiles",
                newName: "Staffs");

            migrationBuilder.RenameIndex(
                name: "IX_InstructorProfiles_AccountId",
                table: "Staffs",
                newName: "IX_Staffs_AccountId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Staffs",
                table: "Staffs",
                column: "Id");

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
                name: "FK_Staffs_Accounts_AccountId",
                table: "Staffs",
                column: "AccountId",
                principalTable: "Accounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
