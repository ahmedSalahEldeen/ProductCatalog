using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProductCatalog.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddImagePath : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ImagePath",
                table: "products",
                newName: "ImageFilePath");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ImageFilePath",
                table: "products",
                newName: "ImagePath");
        }
    }
}
