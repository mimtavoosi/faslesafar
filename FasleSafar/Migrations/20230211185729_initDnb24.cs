using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FasleSafar.Migrations
{
    /// <inheritdoc />
    public partial class initDnb24 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

            migrationBuilder.AddColumn<string>(
                name: "ImageExt",
                table: "Destinations",
                type: "nvarchar(6)",
                maxLength: 6,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ImageExt",
                table: "Contents",
                type: "nvarchar(6)",
                maxLength: 6,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BigImageExt",
                table: "Tours");

            migrationBuilder.DropColumn(
                name: "SmallImageExt",
                table: "Tours");

            migrationBuilder.DropColumn(
                name: "ImageExt",
                table: "Destinations");

            migrationBuilder.DropColumn(
                name: "ImageExt",
                table: "Contents");
        }
    }
}
