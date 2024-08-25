using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MVC_FinalTerm.Migrations
{
    /// <inheritdoc />
    public partial class Migration5 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsOnSales",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "IsOnType",
                table: "Products");

            migrationBuilder.RenameColumn(
                name: "Version",
                table: "Products",
                newName: "StatusName");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "StatusName",
                table: "Products",
                newName: "Version");

            migrationBuilder.AddColumn<bool>(
                name: "IsOnSales",
                table: "Products",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsOnType",
                table: "Products",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
