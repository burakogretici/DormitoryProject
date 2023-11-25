using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KvsProject.Migrations
{
    /// <inheritdoc />
    public partial class mig2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte>(
                name: "UserType",
                table: "Users",
                type: "tinyint",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AlterColumn<byte>(
                name: "UserType",
                table: "Students",
                type: "tinyint",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserType",
                table: "Users");

            migrationBuilder.AlterColumn<int>(
                name: "UserType",
                table: "Students",
                type: "int",
                nullable: false,
                oldClrType: typeof(byte),
                oldType: "tinyint");
        }
    }
}
