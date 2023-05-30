using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MMS.DataService.Migrations
{
    /// <inheritdoc />
    public partial class Add_into_Db_context : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Month_Messes_MessId",
                table: "Month");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Month",
                table: "Month");

            migrationBuilder.RenameTable(
                name: "Month",
                newName: "Months");

            migrationBuilder.RenameIndex(
                name: "IX_Month_MessId",
                table: "Months",
                newName: "IX_Months_MessId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Months",
                table: "Months",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Months_Messes_MessId",
                table: "Months",
                column: "MessId",
                principalTable: "Messes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Months_Messes_MessId",
                table: "Months");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Months",
                table: "Months");

            migrationBuilder.RenameTable(
                name: "Months",
                newName: "Month");

            migrationBuilder.RenameIndex(
                name: "IX_Months_MessId",
                table: "Month",
                newName: "IX_Month_MessId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Month",
                table: "Month",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Month_Messes_MessId",
                table: "Month",
                column: "MessId",
                principalTable: "Messes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
