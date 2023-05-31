using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MMS.DataService.Migrations
{
    /// <inheritdoc />
    public partial class AddAdditional_feild_in_deposit : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "MessId",
                table: "Deposits",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "OrderId",
                table: "Deposits",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<Guid>(
                name: "PersonId",
                table: "Deposits",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Deposits_MessId",
                table: "Deposits",
                column: "MessId");

            migrationBuilder.CreateIndex(
                name: "IX_Deposits_PersonId",
                table: "Deposits",
                column: "PersonId");

            migrationBuilder.AddForeignKey(
                name: "FK_Deposits_Messes_MessId",
                table: "Deposits",
                column: "MessId",
                principalTable: "Messes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Deposits_Persons_PersonId",
                table: "Deposits",
                column: "PersonId",
                principalTable: "Persons",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Deposits_Messes_MessId",
                table: "Deposits");

            migrationBuilder.DropForeignKey(
                name: "FK_Deposits_Persons_PersonId",
                table: "Deposits");

            migrationBuilder.DropIndex(
                name: "IX_Deposits_MessId",
                table: "Deposits");

            migrationBuilder.DropIndex(
                name: "IX_Deposits_PersonId",
                table: "Deposits");

            migrationBuilder.DropColumn(
                name: "MessId",
                table: "Deposits");

            migrationBuilder.DropColumn(
                name: "OrderId",
                table: "Deposits");

            migrationBuilder.DropColumn(
                name: "PersonId",
                table: "Deposits");
        }
    }
}
