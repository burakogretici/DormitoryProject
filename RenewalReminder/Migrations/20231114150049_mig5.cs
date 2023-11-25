using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RenewalReminder.Migrations
{
    /// <inheritdoc />
    public partial class mig5 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Guests",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FullName = table.Column<string>(type: "nvarchar(400)", maxLength: 400, nullable: false),
                    WhyCome = table.Column<string>(type: "nvarchar(400)", maxLength: 400, nullable: false),
                    WhoCome = table.Column<string>(type: "nvarchar(400)", maxLength: 400, nullable: false),
                    FromWhere = table.Column<string>(type: "nvarchar(400)", maxLength: 400, nullable: false),
                    Statu = table.Column<string>(type: "nvarchar(400)", maxLength: 400, nullable: false),
                    Deleted = table.Column<bool>(type: "bit", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Guests", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Guests");
        }
    }
}
