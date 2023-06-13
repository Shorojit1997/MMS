using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MMS.DataService.Migrations
{
    /// <inheritdoc />
    public partial class addcascadedeletemethod : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Expenses_Months_MonthId",
                table: "Expenses");

            migrationBuilder.AddForeignKey(
                name: "FK_Expenses_Months_MonthId",
                table: "Expenses",
                column: "MonthId",
                principalTable: "Months",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Expenses_Months_MonthId",
                table: "Expenses");

            migrationBuilder.AddForeignKey(
                name: "FK_Expenses_Months_MonthId",
                table: "Expenses",
                column: "MonthId",
                principalTable: "Months",
                principalColumn: "Id");
        }
    }
}
