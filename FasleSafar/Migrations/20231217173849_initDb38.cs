using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FasleSafar.Migrations
{
    /// <inheritdoc />
    public partial class initDb38 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Price",
                table: "Tours");

            migrationBuilder.DropColumn(
                name: "Price",
                table: "HotelStarings");

            migrationBuilder.AddColumn<bool>(
                name: "AvaliableOnlinePay",
                table: "Tours",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "FactorRequest",
                table: "Orders",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "AdultPrice",
                table: "HotelStarings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BabyPrice",
                table: "HotelStarings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ChildPrice",
                table: "HotelStarings",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AvaliableOnlinePay",
                table: "Tours");

            migrationBuilder.DropColumn(
                name: "FactorRequest",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "AdultPrice",
                table: "HotelStarings");

            migrationBuilder.DropColumn(
                name: "BabyPrice",
                table: "HotelStarings");

            migrationBuilder.DropColumn(
                name: "ChildPrice",
                table: "HotelStarings");

            migrationBuilder.AddColumn<decimal>(
                name: "Price",
                table: "Tours",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Price",
                table: "HotelStarings",
                type: "decimal(18,2)",
                nullable: true);
        }
    }
}
