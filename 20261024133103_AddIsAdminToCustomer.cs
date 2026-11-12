using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ASP.NET_PlantsOrder.Migrations
{
    /// <inheritdoc />
    public partial class AddIsAdminToCustomer : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "PlantName",
                table: "Plants",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Plants",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsAdmin",
                table: "Customers",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.UpdateData(
                table: "Customers",
                keyColumn: "CustomerId",
                keyValue: 1,
                column: "IsAdmin",
                value: true);

            migrationBuilder.UpdateData(
                table: "Plants",
                keyColumn: "PlantId",
                keyValue: 1,
                columns: new[] { "Description", "ImageUrl" },
                values: new object[] { null, "/PLANT/rose2.jpg" });

            migrationBuilder.UpdateData(
                table: "Plants",
                keyColumn: "PlantId",
                keyValue: 2,
                columns: new[] { "Description", "ImageUrl" },
                values: new object[] { null, "/PLANT/monyplant.jpeg" });

            migrationBuilder.UpdateData(
                table: "Plants",
                keyColumn: "PlantId",
                keyValue: 3,
                columns: new[] { "Description", "ImageUrl" },
                values: new object[] { null, "/PLANT/tulshi.jpeg" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "Plants");

            migrationBuilder.DropColumn(
                name: "IsAdmin",
                table: "Customers");

            migrationBuilder.AlterColumn<string>(
                name: "PlantName",
                table: "Plants",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

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
    }
}
