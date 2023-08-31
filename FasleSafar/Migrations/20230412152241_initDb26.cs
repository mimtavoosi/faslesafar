using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FasleSafar.Migrations
{
    /// <inheritdoc />
    public partial class initDb26 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Clothes",
                table: "Tours",
                newName: "Attractions");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Attractions",
                table: "Tours",
                newName: "Clothes");
        }
    }
}
