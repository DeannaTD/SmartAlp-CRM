using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NewGenBridge.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddGroupLessonField : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "lessonId",
                table: "Groups",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Groups_lessonId",
                table: "Groups",
                column: "lessonId");

            migrationBuilder.AddForeignKey(
                name: "FK_Groups_Lessons_lessonId",
                table: "Groups",
                column: "lessonId",
                principalTable: "Lessons",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Groups_Lessons_lessonId",
                table: "Groups");

            migrationBuilder.DropIndex(
                name: "IX_Groups_lessonId",
                table: "Groups");

            migrationBuilder.DropColumn(
                name: "lessonId",
                table: "Groups");
        }
    }
}
