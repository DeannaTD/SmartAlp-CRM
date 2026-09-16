using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NewGenBridge.Data.Migrations
{
    /// <inheritdoc />
    public partial class TimeTableRemoval : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Groups_TimeTables_TimeTableId",
                table: "Groups");

            migrationBuilder.DropTable(
                name: "TimeTables");

            migrationBuilder.DropIndex(
                name: "IX_Groups_TimeTableId",
                table: "Groups");

            migrationBuilder.RenameColumn(
                name: "TimeTableId",
                table: "Groups",
                newName: "Day");

            migrationBuilder.AddColumn<TimeSpan>(
                name: "Time",
                table: "Groups",
                type: "time",
                nullable: false,
                defaultValue: new TimeSpan(0, 0, 0, 0, 0));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Time",
                table: "Groups");

            migrationBuilder.RenameColumn(
                name: "Day",
                table: "Groups",
                newName: "TimeTableId");

            migrationBuilder.CreateTable(
                name: "TimeTables",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Day = table.Column<int>(type: "int", nullable: false),
                    Time = table.Column<TimeSpan>(type: "time", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TimeTables", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Groups_TimeTableId",
                table: "Groups",
                column: "TimeTableId");

            migrationBuilder.AddForeignKey(
                name: "FK_Groups_TimeTables_TimeTableId",
                table: "Groups",
                column: "TimeTableId",
                principalTable: "TimeTables",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
