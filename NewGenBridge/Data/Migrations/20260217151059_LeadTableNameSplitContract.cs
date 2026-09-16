using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NewGenBridge.Data.Migrations
{
    /// <inheritdoc />
    public partial class LeadTableNameSplitContract : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "StudentName",
                table: "Leads",
                newName: "LastName");

            migrationBuilder.AddColumn<int>(
                name: "ContractId",
                table: "Leads",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FirstName",
                table: "Leads",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ContractId",
                table: "Leads");

            migrationBuilder.DropColumn(
                name: "FirstName",
                table: "Leads");

            migrationBuilder.RenameColumn(
                name: "LastName",
                table: "Leads",
                newName: "StudentName");
        }
    }
}
