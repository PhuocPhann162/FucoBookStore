using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace FucoBook_DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddCompanyTableToDb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Companies",
                columns: new[] { "Id", "City", "Name", "PhoneNumber", "PostalCode", "State", "StreetAddress" },
                values: new object[,]
                {
                    { 1, "Da Nang", "Fahasa", "0899229788", "VN123", "Tan Chinh", "280 Le Duan" },
                    { 2, "Da Nang", "Kim Dong", "0899229788", "VN123", "Tan Chinh", "280 Le Duan" },
                    { 3, "Da Nang", "Tuoi Tre", "0899229788", "VN123", "Tan Chinh", "280 Le Duan" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 3);
        }
    }
}
