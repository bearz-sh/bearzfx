using System;
using System.Diagnostics.CodeAnalysis;

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bearz.Casa.Data.Migrations.Mssql
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "environments",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    displayname = table.Column<string>(name: "display_name", type: "nvarchar(max)", nullable: false),
                    name = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_environments", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "secrets",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    name = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: false),
                    value = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: false),
                    expiresat = table.Column<DateTime>(name: "expires_at", type: "datetime2", nullable: true),
                    createdat = table.Column<DateTime>(name: "created_at", type: "datetime2", nullable: false),
                    updatedat = table.Column<DateTime>(name: "updated_at", type: "datetime2", nullable: true),
                    environmentid = table.Column<int>(name: "environment_id", type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_secrets", x => x.id);
                    table.ForeignKey(
                        name: "fk_secrets_environments_environment_id",
                        column: x => x.environmentid,
                        principalTable: "environments",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "setting",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    name = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: false),
                    value = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: false),
                    environmentid = table.Column<int>(name: "environment_id", type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_setting", x => x.id);
                    table.ForeignKey(
                        name: "fk_setting_environments_environment_id",
                        column: x => x.environmentid,
                        principalTable: "environments",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_secrets_environment_id",
                table: "secrets",
                column: "environment_id");

            migrationBuilder.CreateIndex(
                name: "ix_setting_environment_id",
                table: "setting",
                column: "environment_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "secrets");

            migrationBuilder.DropTable(
                name: "setting");

            migrationBuilder.DropTable(
                name: "environments");
        }
    }
}
