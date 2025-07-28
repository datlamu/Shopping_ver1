using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Shopping_ver1.Migrations
{
    public partial class AddTotalPaymentOrder : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "TotalPayment",
                table: "Orders",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TotalPayment",
                table: "Orders");
        }
    }
}
