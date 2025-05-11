using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace dotnet_store.Migrations
{
    /// <inheritdoc />
    public partial class UpdateProductEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Products",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Image",
                table: "Products",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsHome",
                table: "Products",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Description", "Image", "IsHome" },
                values: new object[] { "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Maecenas a fringilla magna. Duis ullamcorper volutpat nisl ac consequat. Suspendisse in dapibus tortor, at congue tortor.", "1.jpeg", true });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Description", "Image", "IsHome" },
                values: new object[] { "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Maecenas a fringilla magna. Duis ullamcorper volutpat nisl ac consequat. Suspendisse in dapibus tortor, at congue tortor.", "2.jpeg", false });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "Description", "Image", "IsHome" },
                values: new object[] { "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Maecenas a fringilla magna. Duis ullamcorper volutpat nisl ac consequat. Suspendisse in dapibus tortor, at congue tortor.", "3.jpeg", true });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "Image",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "IsHome",
                table: "Products");
        }
    }
}
