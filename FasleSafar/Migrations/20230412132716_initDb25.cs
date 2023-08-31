using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FasleSafar.Migrations
{
    /// <inheritdoc />
    public partial class initDb25 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BigImageExt",
                table: "Tours");

            migrationBuilder.DropColumn(
                name: "SmallImageExt",
                table: "Tours");

            migrationBuilder.AddColumn<string>(
                name: "BigImage",
                table: "Tours",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Clothes",
                table: "Tours",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ExcludeCosts",
                table: "Tours",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Facilities",
                table: "Tours",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ImagesAlbum",
                table: "Tours",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "IncludeCosts",
                table: "Tours",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ReachTime",
                table: "Tours",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ReturnTime",
                table: "Tours",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SmallImage",
                table: "Tours",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BigImage",
                table: "Tours");

            migrationBuilder.DropColumn(
                name: "Clothes",
                table: "Tours");

            migrationBuilder.DropColumn(
                name: "ExcludeCosts",
                table: "Tours");

            migrationBuilder.DropColumn(
                name: "Facilities",
                table: "Tours");

            migrationBuilder.DropColumn(
                name: "ImagesAlbum",
                table: "Tours");

            migrationBuilder.DropColumn(
                name: "IncludeCosts",
                table: "Tours");

            migrationBuilder.DropColumn(
                name: "ReachTime",
                table: "Tours");

            migrationBuilder.DropColumn(
                name: "ReturnTime",
                table: "Tours");

            migrationBuilder.DropColumn(
                name: "SmallImage",
                table: "Tours");

            migrationBuilder.AddColumn<string>(
                name: "BigImageExt",
                table: "Tours",
                type: "nvarchar(6)",
                maxLength: 6,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SmallImageExt",
                table: "Tours",
                type: "nvarchar(6)",
                maxLength: 6,
                nullable: true);
        }
    }
}
