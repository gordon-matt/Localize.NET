using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Localize.Data.Npgsql.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "localize");

            migrationBuilder.CreateTable(
                name: "Languages",
                schema: "localize",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ScopeId = table.Column<string>(type: "character varying(64)", unicode: false, maxLength: 64, nullable: false),
                    Name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    CultureCode = table.Column<string>(type: "character varying(32)", unicode: false, maxLength: 32, nullable: false),
                    IsRightToLeft = table.Column<bool>(type: "boolean", nullable: false),
                    IsEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    SortOrder = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Languages", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LocalizableProperties",
                schema: "localize",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ScopeId = table.Column<string>(type: "character varying(64)", unicode: false, maxLength: 64, nullable: false),
                    CultureCode = table.Column<string>(type: "character varying(32)", unicode: false, maxLength: 32, nullable: false),
                    EntityType = table.Column<string>(type: "character varying(256)", unicode: false, maxLength: 256, nullable: false),
                    EntityId = table.Column<string>(type: "character varying(64)", unicode: false, maxLength: 64, nullable: false),
                    Property = table.Column<string>(type: "character varying(128)", unicode: false, maxLength: 128, nullable: false),
                    Value = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LocalizableProperties", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LocalizableStrings",
                schema: "localize",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ScopeId = table.Column<string>(type: "character varying(64)", unicode: false, maxLength: 64, nullable: false),
                    CultureCode = table.Column<string>(type: "character varying(32)", unicode: false, maxLength: 32, nullable: false),
                    TextKey = table.Column<string>(type: "character varying(400)", maxLength: 400, nullable: false),
                    TextValue = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LocalizableStrings", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Languages_ScopeId_CultureCode",
                schema: "localize",
                table: "Languages",
                columns: new[] { "ScopeId", "CultureCode" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LocalizableProperties_ScopeId_CultureCode_EntityType_Entity~",
                schema: "localize",
                table: "LocalizableProperties",
                columns: new[] { "ScopeId", "CultureCode", "EntityType", "EntityId", "Property" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LocalizableStrings_ScopeId_CultureCode_TextKey",
                schema: "localize",
                table: "LocalizableStrings",
                columns: new[] { "ScopeId", "CultureCode", "TextKey" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Languages",
                schema: "localize");

            migrationBuilder.DropTable(
                name: "LocalizableProperties",
                schema: "localize");

            migrationBuilder.DropTable(
                name: "LocalizableStrings",
                schema: "localize");
        }
    }
}
