using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FasleSafar.Migrations
{
    /// <inheritdoc />
    public partial class initDb19 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PayMode",
                table: "Orders");

            migrationBuilder.RenameColumn(
                name: "Count",
                table: "OrderDetails",
                newName: "ChildCount");

            migrationBuilder.AddColumn<int>(
                name: "AdultCount",
                table: "OrderDetails",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AdultCount",
                table: "OrderDetails");

            migrationBuilder.RenameColumn(
                name: "ChildCount",
                table: "OrderDetails",
                newName: "Count");

            migrationBuilder.AddColumn<string>(
                name: "PayMode",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
