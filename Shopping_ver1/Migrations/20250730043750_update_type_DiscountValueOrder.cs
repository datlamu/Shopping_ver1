using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Shopping_ver1.Migrations
{
    public partial class update_type_DiscountValueOrder : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DiscountVale",
                table: "Orders");

            migrationBuilder.AddColumn<string>(
                name: "DiscountValue",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DiscountValue",
                table: "Orders");

            migrationBuilder.AddColumn<decimal>(
                name: "DiscountVale",
                table: "Orders",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }
    }
}
