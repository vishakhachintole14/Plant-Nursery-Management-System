using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ASP.NET_PlantsOrder.Migrations
{
    /// <inheritdoc />
    public partial class m1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Plants",
                keyColumn: "PlantId",
                keyValue: 1,
                column: "ImageUrl",
                value: "E:/PLANT/rose.jpeg");

            migrationBuilder.UpdateData(
                table: "Plants",
                keyColumn: "PlantId",
                keyValue: 3,
                column: "ImageUrl",
                value: "/PLANT/tulsi.jpeg");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Plants",
                keyColumn: "PlantId",
                keyValue: 1,
                column: "ImageUrl",
                value: "/PLANT/rose2.jpg");

            migrationBuilder.UpdateData(
                table: "Plants",
                keyColumn: "PlantId",
                keyValue: 3,
                column: "ImageUrl",
                value: "/PLANT/tulshi.jpeg");
        }
    }
}
