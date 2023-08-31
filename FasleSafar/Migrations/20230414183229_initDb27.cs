using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FasleSafar.Migrations
{
    /// <inheritdoc />
    public partial class initDb27 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "GeoCoordinates",
                table: "Tours",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GeoCoordinates",
                table: "Tours");
        }
    }
}
