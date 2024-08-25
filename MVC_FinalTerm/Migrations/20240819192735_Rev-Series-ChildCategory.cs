using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MVC_FinalTerm.Migrations
{
    /// <inheritdoc />
    public partial class RevSeriesChildCategory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Products_ChildCategories_ChildCategoryId",
                table: "Products");

            migrationBuilder.DropForeignKey(
                name: "FK_Products_Series_SeriesId",
                table: "Products");

            migrationBuilder.DropTable(
                name: "Series");

            migrationBuilder.DropTable(
                name: "ChildCategories");

            migrationBuilder.DropIndex(
                name: "IX_Products_ChildCategoryId",
                table: "Products");

            migrationBuilder.DropIndex(
                name: "IX_Products_SeriesId",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "ChildCategoryId",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "SeriesId",
                table: "Products");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ChildCategoryId",
                table: "Products",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SeriesId",
                table: "Products",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "ChildCategories",
                columns: table => new
                {
                    ChildCategoryId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CategoryId = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChildCategories", x => x.ChildCategoryId);
                    table.ForeignKey(
                        name: "FK_ChildCategories_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "CategoryId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Series",
                columns: table => new
                {
                    SeriesId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ChildCategoryId = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Series", x => x.SeriesId);
                    table.ForeignKey(
                        name: "FK_Series_ChildCategories_ChildCategoryId",
                        column: x => x.ChildCategoryId,
                        principalTable: "ChildCategories",
                        principalColumn: "ChildCategoryId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Products_ChildCategoryId",
                table: "Products",
                column: "ChildCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Products_SeriesId",
                table: "Products",
                column: "SeriesId");

            migrationBuilder.CreateIndex(
                name: "IX_ChildCategories_CategoryId",
                table: "ChildCategories",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Series_ChildCategoryId",
                table: "Series",
                column: "ChildCategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_Products_ChildCategories_ChildCategoryId",
                table: "Products",
                column: "ChildCategoryId",
                principalTable: "ChildCategories",
                principalColumn: "ChildCategoryId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Products_Series_SeriesId",
                table: "Products",
                column: "SeriesId",
                principalTable: "Series",
                principalColumn: "SeriesId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
