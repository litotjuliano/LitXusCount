using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LitXusCount.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddProductLhdnDefaults : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DefaultLhdnClassificationCode",
                table: "Products",
                type: "character varying(4)",
                maxLength: 4,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DefaultLhdnTaxTypeCode",
                table: "Products",
                type: "character varying(2)",
                maxLength: 2,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DefaultLhdnClassificationCode",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "DefaultLhdnTaxTypeCode",
                table: "Products");
        }
    }
}
