using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NewGenBridge.Data.Migrations
{
    /// <inheritdoc />
    public partial class DivisionMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DivisionId",
                table: "Branches",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Divisions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Divisions", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Branches_DivisionId",
                table: "Branches",
                column: "DivisionId");

            migrationBuilder.AddForeignKey(
                name: "FK_Branches_Divisions_DivisionId",
                table: "Branches",
                column: "DivisionId",
                principalTable: "Divisions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Branches_Divisions_DivisionId",
                table: "Branches");

            migrationBuilder.DropTable(
                name: "Divisions");

            migrationBuilder.DropIndex(
                name: "IX_Branches_DivisionId",
                table: "Branches");

            migrationBuilder.DropColumn(
                name: "DivisionId",
                table: "Branches");
        }
    }
}
