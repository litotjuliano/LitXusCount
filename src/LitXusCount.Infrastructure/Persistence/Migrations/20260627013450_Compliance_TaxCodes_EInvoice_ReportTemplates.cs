using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace LitXusCount.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Compliance_TaxCodes_EInvoice_ReportTemplates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateOnly>(
                name: "BillingPeriodEnd",
                table: "SalesInvoices",
                type: "date",
                nullable: true);

            migrationBuilder.AddColumn<DateOnly>(
                name: "BillingPeriodStart",
                table: "SalesInvoices",
                type: "date",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BuyerAddress",
                table: "SalesInvoices",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BuyerName",
                table: "SalesInvoices",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BuyerRegistrationType",
                table: "SalesInvoices",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BuyerTIN",
                table: "SalesInvoices",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "EInvoiceDateTimeSubmitted",
                table: "SalesInvoices",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "EInvoiceDateTimeValidated",
                table: "SalesInvoices",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EInvoiceLongId",
                table: "SalesInvoices",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "EInvoiceStatus",
                table: "SalesInvoices",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "EInvoiceSubmissionUid",
                table: "SalesInvoices",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EInvoiceUUID",
                table: "SalesInvoices",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "FxRate",
                table: "SalesInvoices",
                type: "numeric",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "GrandTotalMyr",
                table: "SalesInvoices",
                type: "numeric",
                nullable: true);

            migrationBuilder.AddColumn<DateOnly>(
                name: "InvoiceIssueDate",
                table: "SalesInvoices",
                type: "date",
                nullable: true);

            migrationBuilder.AddColumn<TimeOnly>(
                name: "InvoiceIssueTime",
                table: "SalesInvoices",
                type: "time without time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "InvoiceTypeCode",
                table: "SalesInvoices",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsConsolidated",
                table: "SalesInvoices",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "OriginalInvoiceUUID",
                table: "SalesInvoices",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ClassificationCode",
                table: "SalesInvoiceLines",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "TaxExemptionAmount",
                table: "SalesInvoiceLines",
                type: "numeric",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TaxExemptionReason",
                table: "SalesInvoiceLines",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TaxTypeCode",
                table: "SalesInvoiceLines",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UnitCode",
                table: "SalesInvoiceLines",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OriginalEInvoiceUUID",
                table: "ReturnLogs",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RegistrationNumber",
                table: "Customers",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RegistrationType",
                table: "Customers",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SSTRegistrationNumber",
                table: "Customers",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TIN",
                table: "Customers",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MSICCode",
                table: "CompanyInfos",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MSICDescription",
                table: "CompanyInfos",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MyInvoisClientId",
                table: "CompanyInfos",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MyInvoisClientSecret",
                table: "CompanyInfos",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MyInvoisEnvironment",
                table: "CompanyInfos",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RegistrationType",
                table: "CompanyInfos",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SSTRegistrationNumber",
                table: "CompanyInfos",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TIN",
                table: "CompanyInfos",
                type: "text",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "EInvoiceSubmissions",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SalesInvoiceId = table.Column<long>(type: "bigint", nullable: false),
                    SubmissionUid = table.Column<string>(type: "text", nullable: true),
                    ResponseStatus = table.Column<string>(type: "text", nullable: true),
                    ResponseUUID = table.Column<string>(type: "text", nullable: true),
                    ResponseLongId = table.Column<string>(type: "text", nullable: true),
                    RawResponseJson = table.Column<string>(type: "text", nullable: true),
                    SubmittedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    RespondedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EInvoiceSubmissions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EInvoiceSubmissions_SalesInvoices_SalesInvoiceId",
                        column: x => x.SalesInvoiceId,
                        principalTable: "SalesInvoices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TaxCodes",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Code = table.Column<string>(type: "character varying(2)", maxLength: 2, nullable: false),
                    Name = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    Rate = table.Column<decimal>(type: "numeric(8,4)", precision: 8, scale: 4, nullable: false),
                    IsExempt = table.Column<bool>(type: "boolean", nullable: false),
                    IsDefault = table.Column<bool>(type: "boolean", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ModifiedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    ModifiedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaxCodes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TenantReportTemplates",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    DocumentType = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    Name = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    BodyHtml = table.Column<string>(type: "text", nullable: false),
                    IsDefault = table.Column<bool>(type: "boolean", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ModifiedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    ModifiedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TenantReportTemplates", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EInvoiceValidationErrors",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    EInvoiceSubmissionId = table.Column<long>(type: "bigint", nullable: false),
                    ErrorCode = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    ErrorMessage = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: false),
                    PropertyPath = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: true),
                    Target = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EInvoiceValidationErrors", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EInvoiceValidationErrors_EInvoiceSubmissions_EInvoiceSubmis~",
                        column: x => x.EInvoiceSubmissionId,
                        principalTable: "EInvoiceSubmissions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "CompanyInfos",
                keyColumn: "Id",
                keyValue: 1L,
                columns: new[] { "MSICCode", "MSICDescription", "MyInvoisClientId", "MyInvoisClientSecret", "MyInvoisEnvironment", "RegistrationType", "SSTRegistrationNumber", "TIN" },
                values: new object[] { null, null, null, null, null, null, null, null });

            migrationBuilder.CreateIndex(
                name: "IX_EInvoiceSubmissions_SalesInvoiceId",
                table: "EInvoiceSubmissions",
                column: "SalesInvoiceId");

            migrationBuilder.CreateIndex(
                name: "IX_EInvoiceValidationErrors_EInvoiceSubmissionId",
                table: "EInvoiceValidationErrors",
                column: "EInvoiceSubmissionId");

            migrationBuilder.CreateIndex(
                name: "IX_TaxCodes_Code",
                table: "TaxCodes",
                column: "Code",
                unique: true,
                filter: "\"IsActive\" = true");

            migrationBuilder.CreateIndex(
                name: "IX_TenantReportTemplates_DocumentType_IsDefault",
                table: "TenantReportTemplates",
                columns: new[] { "DocumentType", "IsDefault" },
                filter: "\"IsDefault\" = true AND \"IsActive\" = true");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EInvoiceValidationErrors");

            migrationBuilder.DropTable(
                name: "TaxCodes");

            migrationBuilder.DropTable(
                name: "TenantReportTemplates");

            migrationBuilder.DropTable(
                name: "EInvoiceSubmissions");

            migrationBuilder.DropColumn(
                name: "BillingPeriodEnd",
                table: "SalesInvoices");

            migrationBuilder.DropColumn(
                name: "BillingPeriodStart",
                table: "SalesInvoices");

            migrationBuilder.DropColumn(
                name: "BuyerAddress",
                table: "SalesInvoices");

            migrationBuilder.DropColumn(
                name: "BuyerName",
                table: "SalesInvoices");

            migrationBuilder.DropColumn(
                name: "BuyerRegistrationType",
                table: "SalesInvoices");

            migrationBuilder.DropColumn(
                name: "BuyerTIN",
                table: "SalesInvoices");

            migrationBuilder.DropColumn(
                name: "EInvoiceDateTimeSubmitted",
                table: "SalesInvoices");

            migrationBuilder.DropColumn(
                name: "EInvoiceDateTimeValidated",
                table: "SalesInvoices");

            migrationBuilder.DropColumn(
                name: "EInvoiceLongId",
                table: "SalesInvoices");

            migrationBuilder.DropColumn(
                name: "EInvoiceStatus",
                table: "SalesInvoices");

            migrationBuilder.DropColumn(
                name: "EInvoiceSubmissionUid",
                table: "SalesInvoices");

            migrationBuilder.DropColumn(
                name: "EInvoiceUUID",
                table: "SalesInvoices");

            migrationBuilder.DropColumn(
                name: "FxRate",
                table: "SalesInvoices");

            migrationBuilder.DropColumn(
                name: "GrandTotalMyr",
                table: "SalesInvoices");

            migrationBuilder.DropColumn(
                name: "InvoiceIssueDate",
                table: "SalesInvoices");

            migrationBuilder.DropColumn(
                name: "InvoiceIssueTime",
                table: "SalesInvoices");

            migrationBuilder.DropColumn(
                name: "InvoiceTypeCode",
                table: "SalesInvoices");

            migrationBuilder.DropColumn(
                name: "IsConsolidated",
                table: "SalesInvoices");

            migrationBuilder.DropColumn(
                name: "OriginalInvoiceUUID",
                table: "SalesInvoices");

            migrationBuilder.DropColumn(
                name: "ClassificationCode",
                table: "SalesInvoiceLines");

            migrationBuilder.DropColumn(
                name: "TaxExemptionAmount",
                table: "SalesInvoiceLines");

            migrationBuilder.DropColumn(
                name: "TaxExemptionReason",
                table: "SalesInvoiceLines");

            migrationBuilder.DropColumn(
                name: "TaxTypeCode",
                table: "SalesInvoiceLines");

            migrationBuilder.DropColumn(
                name: "UnitCode",
                table: "SalesInvoiceLines");

            migrationBuilder.DropColumn(
                name: "OriginalEInvoiceUUID",
                table: "ReturnLogs");

            migrationBuilder.DropColumn(
                name: "RegistrationNumber",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "RegistrationType",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "SSTRegistrationNumber",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "TIN",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "MSICCode",
                table: "CompanyInfos");

            migrationBuilder.DropColumn(
                name: "MSICDescription",
                table: "CompanyInfos");

            migrationBuilder.DropColumn(
                name: "MyInvoisClientId",
                table: "CompanyInfos");

            migrationBuilder.DropColumn(
                name: "MyInvoisClientSecret",
                table: "CompanyInfos");

            migrationBuilder.DropColumn(
                name: "MyInvoisEnvironment",
                table: "CompanyInfos");

            migrationBuilder.DropColumn(
                name: "RegistrationType",
                table: "CompanyInfos");

            migrationBuilder.DropColumn(
                name: "SSTRegistrationNumber",
                table: "CompanyInfos");

            migrationBuilder.DropColumn(
                name: "TIN",
                table: "CompanyInfos");
        }
    }
}
