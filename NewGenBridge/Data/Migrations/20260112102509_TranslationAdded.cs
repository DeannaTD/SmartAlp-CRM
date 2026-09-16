using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NewGenBridge.Data.Migrations
{
    /// <inheritdoc />
    public partial class TranslationAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Translations",
                columns: table => new
                {
                    NPKey = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    EnValue = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RuValue = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TrValue = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FrValue = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ItValue = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DeValue = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Translations", x => x.NPKey);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Translations");
        }
    }
}
