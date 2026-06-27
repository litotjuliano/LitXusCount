using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LitXusCount.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class BugFixes_InvoiceSequence_InvoiceNoIndex_VatDecimal : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "Percentage",
                table: "VatPercentages",
                type: "numeric(8,4)",
                precision: 8,
                scale: 4,
                nullable: false,
                oldClrType: typeof(double),
                oldType: "double precision");

            migrationBuilder.CreateTable(
                name: "InvoiceSequences",
                columns: table => new
                {
                    Category = table.Column<int>(type: "integer", nullable: false),
                    LastNumber = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InvoiceSequences", x => x.Category);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SalesInvoices_InvoiceNo",
                table: "SalesInvoices",
                column: "InvoiceNo",
                unique: true,
                filter: "\"IsActive\" = true");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "InvoiceSequences");

            migrationBuilder.DropIndex(
                name: "IX_SalesInvoices_InvoiceNo",
                table: "SalesInvoices");

            migrationBuilder.AlterColumn<double>(
                name: "Percentage",
                table: "VatPercentages",
                type: "double precision",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(8,4)",
                oldPrecision: 8,
                oldScale: 4);
        }
    }
}
