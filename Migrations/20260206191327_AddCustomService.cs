using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WorkShopManager.Migrations
{
    /// <inheritdoc />
    public partial class AddCustomService : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CustomServicesCsv",
                table: "AspNetUsers",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CustomServicesCsv",
                table: "AspNetUsers");
        }
    }
}
