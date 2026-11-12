using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ASP.NET_PlantsOrder.Migrations
{
    /// <inheritdoc />
    public partial class UpdatePlantImageUrl : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "ImageUrl",
                table: "Plants",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.UpdateData(
                table: "Plants",
                keyColumn: "PlantId",
                keyValue: 1,
                column: "ImageUrl",
                value: "\"E:\\PLANTS\\Rose.jpeg\"");

            migrationBuilder.UpdateData(
                table: "Plants",
                keyColumn: "PlantId",
                keyValue: 2,
                column: "ImageUrl",
                value: "\"E:\\PLANTS\\monyplant.jpeg\"");

            migrationBuilder.UpdateData(
                table: "Plants",
                keyColumn: "PlantId",
                keyValue: 3,
                column: "ImageUrl",
                value: "\"E:\\PLANTS\\tulshi.jpeg\"");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "ImageUrl",
                table: "Plants",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.UpdateData(
                table: "Plants",
                keyColumn: "PlantId",
                keyValue: 1,
                column: "ImageUrl",
                value: "rose.jpg");

            migrationBuilder.UpdateData(
                table: "Plants",
                keyColumn: "PlantId",
                keyValue: 2,
                column: "ImageUrl",
                value: "moneyplant.jpg");

            migrationBuilder.UpdateData(
                table: "Plants",
                keyColumn: "PlantId",
                keyValue: 3,
                column: "ImageUrl",
                value: "tulsi.jpg");
        }
    }
}
