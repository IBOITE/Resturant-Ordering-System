using Microsoft.EntityFrameworkCore.Migrations;

namespace Lokanta.Data.Migrations
{
    public partial class AddMenuItemID : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MenuItemId",
                table: "OrderDetails",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MenuItemId",
                table: "OrderDetails");
        }
    }
}
