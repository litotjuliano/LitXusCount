using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace LitXusCount.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class LHDN_ClassificationCodes_PaymentType_UOM : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UnCefactCode",
                table: "UnitsOfMeasure",
                type: "character varying(8)",
                maxLength: 8,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LhdnCode",
                table: "PaymentTypes",
                type: "character varying(4)",
                maxLength: 4,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "LhdnClassificationCodes",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Code = table.Column<string>(type: "character varying(4)", maxLength: 4, nullable: false),
                    Description = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ModifiedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    ModifiedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LhdnClassificationCodes", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LhdnClassificationCodes_Code",
                table: "LhdnClassificationCodes",
                column: "Code",
                unique: true,
                filter: "\"IsActive\" = true");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LhdnClassificationCodes");

            migrationBuilder.DropColumn(
                name: "UnCefactCode",
                table: "UnitsOfMeasure");

            migrationBuilder.DropColumn(
                name: "LhdnCode",
                table: "PaymentTypes");
        }
    }
}
