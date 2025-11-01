using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TutorDrive.Migrations
{
    /// <inheritdoc />
    public partial class updatedb1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Comment",
                table: "LearningProgresses",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<DateTime>(
                name: "EndDate",
                table: "LearningProgresses",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsCompleted",
                table: "LearningProgresses",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastUpdated",
                table: "LearningProgresses",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "StaffId",
                table: "LearningProgresses",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "StartDate",
                table: "LearningProgresses",
                type: "datetime2",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_LearningProgresses_StaffId",
                table: "LearningProgresses",
                column: "StaffId");

            migrationBuilder.AddForeignKey(
                name: "FK_LearningProgresses_Staffs_StaffId",
                table: "LearningProgresses",
                column: "StaffId",
                principalTable: "Staffs",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LearningProgresses_Staffs_StaffId",
                table: "LearningProgresses");

            migrationBuilder.DropIndex(
                name: "IX_LearningProgresses_StaffId",
                table: "LearningProgresses");

            migrationBuilder.DropColumn(
                name: "EndDate",
                table: "LearningProgresses");

            migrationBuilder.DropColumn(
                name: "IsCompleted",
                table: "LearningProgresses");

            migrationBuilder.DropColumn(
                name: "LastUpdated",
                table: "LearningProgresses");

            migrationBuilder.DropColumn(
                name: "StaffId",
                table: "LearningProgresses");

            migrationBuilder.DropColumn(
                name: "StartDate",
                table: "LearningProgresses");

            migrationBuilder.AlterColumn<int>(
                name: "Comment",
                table: "LearningProgresses",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);
        }
    }
}
