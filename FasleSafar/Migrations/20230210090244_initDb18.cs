using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FasleSafar.Migrations
{
    /// <inheritdoc />
    public partial class initDb18 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsLeasing",
                table: "Tours",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "PayMode",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsLeasing",
                table: "Tours");

            migrationBuilder.DropColumn(
                name: "PayMode",
                table: "Orders");
        }
    }
}
