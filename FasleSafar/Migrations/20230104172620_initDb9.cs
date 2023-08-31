using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FasleSafar.Migrations
{
    /// <inheritdoc />
    public partial class initDb9 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ContentKey",
                table: "Contents");

            migrationBuilder.AddColumn<bool>(
                name: "HasImage",
                table: "Contents",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HasImage",
                table: "Contents");

            migrationBuilder.AddColumn<string>(
                name: "ContentKey",
                table: "Contents",
                type: "nvarchar(60)",
                maxLength: 60,
                nullable: true);
        }
    }
}
