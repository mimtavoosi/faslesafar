using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FasleSafar.Migrations
{
    /// <inheritdoc />
    public partial class initDb23 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_HotelStarings_Users_UserId",
                table: "HotelStarings");

            migrationBuilder.DropForeignKey(
                name: "FK_RatingHistories_Tours_TourId",
                table: "RatingHistories");

            migrationBuilder.DropForeignKey(
                name: "FK_RatingHistories_Users_UserId",
                table: "RatingHistories");

            migrationBuilder.DropIndex(
                name: "IX_RatingHistories_TourId",
                table: "RatingHistories");

            migrationBuilder.DropIndex(
                name: "IX_RatingHistories_UserId",
                table: "RatingHistories");

            migrationBuilder.DropIndex(
                name: "IX_HotelStarings_UserId",
                table: "HotelStarings");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "HotelStarings");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "HotelStarings",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_RatingHistories_TourId",
                table: "RatingHistories",
                column: "TourId");

            migrationBuilder.CreateIndex(
                name: "IX_RatingHistories_UserId",
                table: "RatingHistories",
                column: "UserId");

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

            migrationBuilder.AddForeignKey(
                name: "FK_RatingHistories_Tours_TourId",
                table: "RatingHistories",
                column: "TourId",
                principalTable: "Tours",
                principalColumn: "TourId");

            migrationBuilder.AddForeignKey(
                name: "FK_RatingHistories_Users_UserId",
                table: "RatingHistories",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "UserId");
        }
    }
}
