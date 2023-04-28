using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bearz.Casa.Data.Migrations.Sqlite
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
                    id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    displayname = table.Column<string>(name: "display_name", type: "TEXT", nullable: false),
                    name = table.Column<string>(type: "TEXT", maxLength: 128, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_environments", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "secrets",
                columns: table => new
                {
                    id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    name = table.Column<string>(type: "TEXT", maxLength: 512, nullable: false),
                    value = table.Column<string>(type: "TEXT", maxLength: 2048, nullable: false),
                    expiresat = table.Column<DateTime>(name: "expires_at", type: "TEXT", nullable: true),
                    createdat = table.Column<DateTime>(name: "created_at", type: "TEXT", nullable: false),
                    updatedat = table.Column<DateTime>(name: "updated_at", type: "TEXT", nullable: true),
                    environmentid = table.Column<int>(name: "environment_id", type: "INTEGER", nullable: false)
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
                    id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    name = table.Column<string>(type: "TEXT", maxLength: 512, nullable: false),
                    value = table.Column<string>(type: "TEXT", maxLength: 2048, nullable: false),
                    environmentid = table.Column<int>(name: "environment_id", type: "INTEGER", nullable: false)
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
