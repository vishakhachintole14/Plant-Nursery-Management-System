using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ASP.NET_PlantsOrder.Migrations
{
    /// <inheritdoc />
    public partial class AddPasswordHashToCustomer : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PasswordHash",
                table: "Customers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "Customers",
                keyColumn: "CustomerId",
                keyValue: 1,
                columns: new[] { "Address", "Email", "PasswordHash", "Phone" },
                values: new object[] { null, "admin@example.com", "REPLACE_WITH_A_VALID_HASH", null });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PasswordHash",
                table: "Customers");

            migrationBuilder.UpdateData(
                table: "Customers",
                keyColumn: "CustomerId",
                keyValue: 1,
                columns: new[] { "Address", "Email", "Phone" },
                values: new object[] { "Pune", "Admin@gmail.com", "7896541236" });
        }
    }
}
