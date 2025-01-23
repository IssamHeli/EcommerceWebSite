using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EcommercApi.Migrations
{
    /// <inheritdoc />
    public partial class returntiinitialproductmodel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImagesUrl",
                table: "Products");

            migrationBuilder.AddColumn<string>(
                name: "ImageUrl",
                table: "Products",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageUrl",
                table: "Products");

            migrationBuilder.AddColumn<string>(
                name: "ImagesUrl",
                table: "Products",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
