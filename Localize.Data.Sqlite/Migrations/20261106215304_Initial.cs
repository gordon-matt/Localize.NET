using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Localize.Data.Sqlite.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Localize_Languages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ScopeId = table.Column<string>(type: "TEXT", unicode: false, maxLength: 64, nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    CultureCode = table.Column<string>(type: "TEXT", unicode: false, maxLength: 32, nullable: false),
                    IsRightToLeft = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsEnabled = table.Column<bool>(type: "INTEGER", nullable: false),
                    SortOrder = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Localize_Languages", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Localize_LocalizableProperties",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ScopeId = table.Column<string>(type: "TEXT", unicode: false, maxLength: 64, nullable: false),
                    CultureCode = table.Column<string>(type: "TEXT", unicode: false, maxLength: 32, nullable: false),
                    EntityType = table.Column<string>(type: "TEXT", unicode: false, maxLength: 256, nullable: false),
                    EntityId = table.Column<string>(type: "TEXT", unicode: false, maxLength: 64, nullable: false),
                    Property = table.Column<string>(type: "TEXT", unicode: false, maxLength: 128, nullable: false),
                    Value = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Localize_LocalizableProperties", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Localize_LocalizableStrings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ScopeId = table.Column<string>(type: "TEXT", unicode: false, maxLength: 64, nullable: false),
                    CultureCode = table.Column<string>(type: "TEXT", unicode: false, maxLength: 32, nullable: false),
                    TextKey = table.Column<string>(type: "TEXT", maxLength: 400, nullable: false),
                    TextValue = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Localize_LocalizableStrings", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Localize_Languages_ScopeId_CultureCode",
                table: "Localize_Languages",
                columns: new[] { "ScopeId", "CultureCode" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Localize_LocalizableProperties_ScopeId_CultureCode_EntityType_EntityId_Property",
                table: "Localize_LocalizableProperties",
                columns: new[] { "ScopeId", "CultureCode", "EntityType", "EntityId", "Property" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Localize_LocalizableStrings_ScopeId_CultureCode_TextKey",
                table: "Localize_LocalizableStrings",
                columns: new[] { "ScopeId", "CultureCode", "TextKey" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Localize_Languages");

            migrationBuilder.DropTable(
                name: "Localize_LocalizableProperties");

            migrationBuilder.DropTable(
                name: "Localize_LocalizableStrings");
        }
    }
}
