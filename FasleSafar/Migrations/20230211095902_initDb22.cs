using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FasleSafar.Migrations
{
    /// <inheritdoc />
    public partial class initDb22 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "HotelStarings",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_HotelStarings_UserId",
                table: "HotelStarings",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_HotelStarings_Users_UserId",
                table: "HotelStarings",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_HotelStarings_Users_UserId",
                table: "HotelStarings");

            migrationBuilder.DropIndex(
                name: "IX_HotelStarings_UserId",
                table: "HotelStarings");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "HotelStarings");
        }
    }
}
