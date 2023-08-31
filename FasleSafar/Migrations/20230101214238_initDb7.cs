using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FasleSafar.Migrations
{
    /// <inheritdoc />
    public partial class initDb7 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OpenStatus",
                table: "Tours");

            migrationBuilder.AddColumn<string>(
                name: "OpenState",
                table: "Tours",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OpenState",
                table: "Tours");

            migrationBuilder.AddColumn<bool>(
                name: "OpenStatus",
                table: "Tours",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
