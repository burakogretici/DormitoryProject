using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RenewalReminder.Migrations
{
    /// <inheritdoc />
    public partial class mig12 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "Guests");

            migrationBuilder.AlterColumn<int>(
                name: "ToWhere",
                table: "Permissions",
                type: "int",
                maxLength: 400,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(400)",
                oldMaxLength: 400,
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "GuestType",
                table: "Guests",
                type: "int",
                maxLength: 400,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GuestType",
                table: "Guests");

            migrationBuilder.AlterColumn<string>(
                name: "ToWhere",
                table: "Permissions",
                type: "nvarchar(400)",
                maxLength: 400,
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldMaxLength: 400,
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "Guests",
                type: "nvarchar(400)",
                maxLength: 400,
                nullable: true);
        }
    }
}
