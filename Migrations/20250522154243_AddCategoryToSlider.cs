using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace dotnet_store.Migrations
{
    /// <inheritdoc />
    public partial class AddCategoryToSlider : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Sliders",
                keyColumn: "Id",
                keyValue: 1,
                column: "CategoryId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "Sliders",
                keyColumn: "Id",
                keyValue: 2,
                column: "CategoryId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "Sliders",
                keyColumn: "Id",
                keyValue: 3,
                column: "CategoryId",
                value: 1);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Sliders",
                keyColumn: "Id",
                keyValue: 1,
                column: "CategoryId",
                value: 0);

            migrationBuilder.UpdateData(
                table: "Sliders",
                keyColumn: "Id",
                keyValue: 2,
                column: "CategoryId",
                value: 0);

            migrationBuilder.UpdateData(
                table: "Sliders",
                keyColumn: "Id",
                keyValue: 3,
                column: "CategoryId",
                value: 0);
        }
    }
}
