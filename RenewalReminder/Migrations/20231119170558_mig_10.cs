using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KvsProject.Migrations
{
    /// <inheritdoc />
    public partial class mig10 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Centrals_Students_StudentId",
                table: "Centrals");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Centrals",
                table: "Centrals");

            migrationBuilder.RenameTable(
                name: "Centrals",
                newName: "Permissions");

            migrationBuilder.RenameIndex(
                name: "IX_Centrals_StudentId",
                table: "Permissions",
                newName: "IX_Permissions_StudentId");

            migrationBuilder.AlterColumn<string>(
                name: "ToWhere",
                table: "Permissions",
                type: "nvarchar(400)",
                maxLength: 400,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(400)",
                oldMaxLength: 400);

            migrationBuilder.AlterColumn<int>(
                name: "Staff",
                table: "Permissions",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CheckOutTime",
                table: "Permissions",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AddColumn<string>(
                name: "Discriminator",
                table: "Permissions",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Permissions",
                table: "Permissions",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Permissions_Students_StudentId",
                table: "Permissions",
                column: "StudentId",
                principalTable: "Students",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Permissions_Students_StudentId",
                table: "Permissions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Permissions",
                table: "Permissions");

            migrationBuilder.DropColumn(
                name: "Discriminator",
                table: "Permissions");

            migrationBuilder.RenameTable(
                name: "Permissions",
                newName: "Centrals");

            migrationBuilder.RenameIndex(
                name: "IX_Permissions_StudentId",
                table: "Centrals",
                newName: "IX_Centrals_StudentId");

            migrationBuilder.AlterColumn<string>(
                name: "ToWhere",
                table: "Centrals",
                type: "nvarchar(400)",
                maxLength: 400,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(400)",
                oldMaxLength: 400,
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Staff",
                table: "Centrals",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CheckOutTime",
                table: "Centrals",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Centrals",
                table: "Centrals",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Centrals_Students_StudentId",
                table: "Centrals",
                column: "StudentId",
                principalTable: "Students",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
