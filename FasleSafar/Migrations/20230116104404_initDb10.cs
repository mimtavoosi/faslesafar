using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FasleSafar.Migrations
{
    /// <inheritdoc />
    public partial class initDb10 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HotelName",
                table: "Tours");

            migrationBuilder.DropColumn(
                name: "Rate",
                table: "Destinations");

            migrationBuilder.RenameColumn(
                name: "Rate",
                table: "Tours",
                newName: "TotalScore");

            migrationBuilder.AddColumn<float>(
                name: "AvgScore",
                table: "Tours",
                type: "real",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<int>(
                name: "ScoreCount",
                table: "Tours",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "HotelStarings",
                columns: table => new
                {
                    StaringId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TourId = table.Column<int>(type: "int", nullable: false),
                    Price = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HotelStarings", x => x.StaringId);
                    table.ForeignKey(
                        name: "FK_HotelStarings_Tours_TourId",
                        column: x => x.TourId,
                        principalTable: "Tours",
                        principalColumn: "TourId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_HotelStarings_TourId",
                table: "HotelStarings",
                column: "TourId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HotelStarings");

            migrationBuilder.DropColumn(
                name: "AvgScore",
                table: "Tours");

            migrationBuilder.DropColumn(
                name: "ScoreCount",
                table: "Tours");

            migrationBuilder.RenameColumn(
                name: "TotalScore",
                table: "Tours",
                newName: "Rate");

            migrationBuilder.AddColumn<string>(
                name: "HotelName",
                table: "Tours",
                type: "nvarchar(60)",
                maxLength: 60,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<float>(
                name: "Rate",
                table: "Destinations",
                type: "real",
                nullable: false,
                defaultValue: 0f);
        }
    }
}
