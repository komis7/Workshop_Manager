using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WorkShopManager.Migrations
{
    /// <inheritdoc />
    public partial class AddWorkshopOpenDays : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "OpenDaysCsv",
                table: "AspNetUsers",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OpenDaysCsv",
                table: "AspNetUsers");
        }
    }
}
