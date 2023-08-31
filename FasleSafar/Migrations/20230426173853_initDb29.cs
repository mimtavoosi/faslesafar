using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FasleSafar.Migrations
{
    /// <inheritdoc />
    public partial class initDb29 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageExt",
                table: "Destinations");

            migrationBuilder.AddColumn<string>(
                name: "BigImage",
                table: "Destinations",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "City",
                table: "Destinations",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Country",
                table: "Destinations",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GeoCoordinates",
                table: "Destinations",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ImagesAlbum",
                table: "Destinations",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsAttraction",
                table: "Destinations",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "OnVitrin",
                table: "Destinations",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Province",
                table: "Destinations",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Attractions",
                columns: table => new
                {
                    AttractionId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AttractionName = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false),
                    AttractionDescription = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BigImage = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ImagesAlbum = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GeoCoordinates = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DestinationId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Attractions", x => x.AttractionId);
                    table.ForeignKey(
                        name: "FK_Attractions_Destinations_DestinationId",
                        column: x => x.DestinationId,
                        principalTable: "Destinations",
                        principalColumn: "DestinationId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Attractions_DestinationId",
                table: "Attractions",
                column: "DestinationId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Attractions");

            migrationBuilder.DropColumn(
                name: "BigImage",
                table: "Destinations");

            migrationBuilder.DropColumn(
                name: "City",
                table: "Destinations");

            migrationBuilder.DropColumn(
                name: "Country",
                table: "Destinations");

            migrationBuilder.DropColumn(
                name: "GeoCoordinates",
                table: "Destinations");

            migrationBuilder.DropColumn(
                name: "ImagesAlbum",
                table: "Destinations");

            migrationBuilder.DropColumn(
                name: "IsAttraction",
                table: "Destinations");

            migrationBuilder.DropColumn(
                name: "OnVitrin",
                table: "Destinations");

            migrationBuilder.DropColumn(
                name: "Province",
                table: "Destinations");

            migrationBuilder.AddColumn<string>(
                name: "ImageExt",
                table: "Destinations",
                type: "nvarchar(6)",
                maxLength: 6,
                nullable: true);
        }
    }
}
