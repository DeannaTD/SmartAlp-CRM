using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NewGenBridge.Data.Migrations
{
    /// <inheritdoc />
    public partial class UserAccessNamingMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserAccesses_Branches_branchId",
                table: "UserAccesses");

            migrationBuilder.DropForeignKey(
                name: "FK_UserAccesses_Divisions_divisionId",
                table: "UserAccesses");

            migrationBuilder.RenameColumn(
                name: "divisionId",
                table: "UserAccesses",
                newName: "DivisionId");

            migrationBuilder.RenameColumn(
                name: "branchId",
                table: "UserAccesses",
                newName: "BranchId");

            migrationBuilder.RenameIndex(
                name: "IX_UserAccesses_divisionId",
                table: "UserAccesses",
                newName: "IX_UserAccesses_DivisionId");

            migrationBuilder.RenameIndex(
                name: "IX_UserAccesses_branchId",
                table: "UserAccesses",
                newName: "IX_UserAccesses_BranchId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserAccesses_Branches_BranchId",
                table: "UserAccesses",
                column: "BranchId",
                principalTable: "Branches",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UserAccesses_Divisions_DivisionId",
                table: "UserAccesses",
                column: "DivisionId",
                principalTable: "Divisions",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserAccesses_Branches_BranchId",
                table: "UserAccesses");

            migrationBuilder.DropForeignKey(
                name: "FK_UserAccesses_Divisions_DivisionId",
                table: "UserAccesses");

            migrationBuilder.RenameColumn(
                name: "DivisionId",
                table: "UserAccesses",
                newName: "divisionId");

            migrationBuilder.RenameColumn(
                name: "BranchId",
                table: "UserAccesses",
                newName: "branchId");

            migrationBuilder.RenameIndex(
                name: "IX_UserAccesses_DivisionId",
                table: "UserAccesses",
                newName: "IX_UserAccesses_divisionId");

            migrationBuilder.RenameIndex(
                name: "IX_UserAccesses_BranchId",
                table: "UserAccesses",
                newName: "IX_UserAccesses_branchId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserAccesses_Branches_branchId",
                table: "UserAccesses",
                column: "branchId",
                principalTable: "Branches",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UserAccesses_Divisions_divisionId",
                table: "UserAccesses",
                column: "divisionId",
                principalTable: "Divisions",
                principalColumn: "Id");
        }
    }
}
