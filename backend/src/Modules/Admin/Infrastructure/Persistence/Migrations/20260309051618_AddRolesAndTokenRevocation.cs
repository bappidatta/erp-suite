using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace ErpSuite.Modules.Admin.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddRolesAndTokenRevocation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "must_change_password",
                schema: "public",
                table: "users",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<long>(
                name: "role_id",
                schema: "public",
                table: "users",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateTable(
                name: "revoked_tokens",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    jti = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    user_id = table.Column<long>(type: "bigint", nullable: false),
                    expires_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    updated_by = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    version = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_revoked_tokens", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "roles",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    description = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    updated_by = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    version = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_roles", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_users_role_id",
                schema: "public",
                table: "users",
                column: "role_id");

            migrationBuilder.CreateIndex(
                name: "IX_revoked_tokens_expires_at",
                schema: "public",
                table: "revoked_tokens",
                column: "expires_at");

            migrationBuilder.CreateIndex(
                name: "IX_revoked_tokens_jti",
                schema: "public",
                table: "revoked_tokens",
                column: "jti",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_roles_name",
                schema: "public",
                table: "roles",
                column: "name",
                unique: true);

            // Seed default roles and assign existing users before adding FK
            migrationBuilder.Sql(@"
                INSERT INTO public.roles (name, description, created_at, created_by, version)
                VALUES ('Admin', 'Full system administrator', NOW(), 'system', 0),
                       ('User', 'Standard user', NOW(), 'system', 0)
                ON CONFLICT (name) DO NOTHING;
            ");

            migrationBuilder.Sql(@"
                UPDATE public.users
                SET role_id = (SELECT id FROM public.roles WHERE name = 'Admin')
                WHERE email = 'admin@erpsuite.local' AND role_id = 0;
            ");

            migrationBuilder.Sql(@"
                UPDATE public.users
                SET role_id = (SELECT id FROM public.roles WHERE name = 'User')
                WHERE role_id = 0;
            ");

            migrationBuilder.AddForeignKey(
                name: "FK_users_roles_role_id",
                schema: "public",
                table: "users",
                column: "role_id",
                principalSchema: "public",
                principalTable: "roles",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_users_roles_role_id",
                schema: "public",
                table: "users");

            migrationBuilder.DropTable(
                name: "revoked_tokens",
                schema: "public");

            migrationBuilder.DropTable(
                name: "roles",
                schema: "public");

            migrationBuilder.DropIndex(
                name: "IX_users_role_id",
                schema: "public",
                table: "users");

            migrationBuilder.DropColumn(
                name: "must_change_password",
                schema: "public",
                table: "users");

            migrationBuilder.DropColumn(
                name: "role_id",
                schema: "public",
                table: "users");
        }
    }
}
