using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FasleSafar.Migrations
{
    /// <inheritdoc />
    public partial class initDb13 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "HotelStarings",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Title",
                table: "HotelStarings");
        }
    }
}
