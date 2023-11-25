using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RenewalReminder.Migrations
{
    /// <inheritdoc />
    public partial class mig3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserType",
                table: "Students");

            migrationBuilder.AddColumn<int>(
                name: "StudentType",
                table: "Students",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StudentType",
                table: "Students");

            migrationBuilder.AddColumn<byte>(
                name: "UserType",
                table: "Students",
                type: "tinyint",
                nullable: false,
                defaultValue: (byte)0);
        }
    }
}
