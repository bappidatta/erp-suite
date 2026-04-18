using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace ErpSuite.Modules.Admin.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ImplementNextStep : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "financial_periods",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    start_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    end_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false),
                    closed_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    closed_by = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    updated_by = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    version = table.Column<int>(type: "integer", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    deleted_by = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_financial_periods", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "journal_entries",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    number = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    entry_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    reference = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    status = table.Column<int>(type: "integer", nullable: false),
                    posted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    posted_by = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    total_debit = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    total_credit = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    updated_by = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    version = table.Column<int>(type: "integer", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    deleted_by = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_journal_entries", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "number_sequences",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    module = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    document_type = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    prefix = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    suffix = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    starting_number = table.Column<int>(type: "integer", nullable: false),
                    next_number = table.Column<int>(type: "integer", nullable: false),
                    padding_length = table.Column<int>(type: "integer", nullable: false),
                    increment_by = table.Column<int>(type: "integer", nullable: false),
                    reset_policy = table.Column<int>(type: "integer", nullable: false),
                    last_reset_on = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    is_active = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    updated_by = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    version = table.Column<int>(type: "integer", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    deleted_by = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_number_sequences", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "journal_entry_lines",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    journal_entry_id = table.Column<long>(type: "bigint", nullable: false),
                    line_number = table.Column<int>(type: "integer", nullable: false),
                    account_id = table.Column<long>(type: "bigint", nullable: false),
                    description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    debit_amount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    credit_amount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    updated_by = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    version = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_journal_entry_lines", x => x.id);
                    table.ForeignKey(
                        name: "FK_journal_entry_lines_accounts_account_id",
                        column: x => x.account_id,
                        principalSchema: "public",
                        principalTable: "accounts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_journal_entry_lines_journal_entries_journal_entry_id",
                        column: x => x.journal_entry_id,
                        principalSchema: "public",
                        principalTable: "journal_entries",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_financial_periods_start_date_end_date",
                schema: "public",
                table: "financial_periods",
                columns: new[] { "start_date", "end_date" });

            migrationBuilder.CreateIndex(
                name: "IX_journal_entries_entry_date",
                schema: "public",
                table: "journal_entries",
                column: "entry_date");

            migrationBuilder.CreateIndex(
                name: "IX_journal_entries_number",
                schema: "public",
                table: "journal_entries",
                column: "number",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_journal_entry_lines_account_id",
                schema: "public",
                table: "journal_entry_lines",
                column: "account_id");

            migrationBuilder.CreateIndex(
                name: "IX_journal_entry_lines_journal_entry_id",
                schema: "public",
                table: "journal_entry_lines",
                column: "journal_entry_id");

            migrationBuilder.CreateIndex(
                name: "IX_number_sequences_module_document_type",
                schema: "public",
                table: "number_sequences",
                columns: new[] { "module", "document_type" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "financial_periods",
                schema: "public");

            migrationBuilder.DropTable(
                name: "journal_entry_lines",
                schema: "public");

            migrationBuilder.DropTable(
                name: "number_sequences",
                schema: "public");

            migrationBuilder.DropTable(
                name: "journal_entries",
                schema: "public");
        }
    }
}
