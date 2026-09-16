using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NewGenBridge.Data.Migrations
{
    /// <inheritdoc />
    public partial class TotalAddedToRoyalty : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "Total",
                table: "Royalties",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Total",
                table: "Royalties");
        }
    }
}
