using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NewGenBridge.Data.Migrations
{
    /// <inheritdoc />
    public partial class CompanyAddressAdd : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Address",
                table: "Companies",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_KitRequests_CompanyId",
                table: "KitRequests",
                column: "CompanyId");

            migrationBuilder.AddForeignKey(
                name: "FK_KitRequests_Companies_CompanyId",
                table: "KitRequests",
                column: "CompanyId",
                principalTable: "Companies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_KitRequests_Companies_CompanyId",
                table: "KitRequests");

            migrationBuilder.DropIndex(
                name: "IX_KitRequests_CompanyId",
                table: "KitRequests");

            migrationBuilder.DropColumn(
                name: "Address",
                table: "Companies");
        }
    }
}
