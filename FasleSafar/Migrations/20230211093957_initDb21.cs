using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FasleSafar.Migrations
{
    /// <inheritdoc />
    public partial class initDb21 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "RatingHistories",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "TourId",
                table: "RatingHistories",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.CreateIndex(
                name: "IX_RatingHistories_TourId",
                table: "RatingHistories",
                column: "TourId");

            migrationBuilder.CreateIndex(
                name: "IX_RatingHistories_UserId",
                table: "RatingHistories",
                column: "UserId");

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
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

            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "RatingHistories",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "TourId",
                table: "RatingHistories",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);
        }
    }
}
