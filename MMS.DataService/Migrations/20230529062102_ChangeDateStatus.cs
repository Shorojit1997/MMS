using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MMS.DataService.Migrations
{
    /// <inheritdoc />
    public partial class ChangeDateStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UpdateDate",
                table: "Persons",
                newName: "UpdateAt");

            migrationBuilder.RenameColumn(
                name: "AddedDate",
                table: "Persons",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "UpdateDate",
                table: "MessHaveMembers",
                newName: "UpdateAt");

            migrationBuilder.RenameColumn(
                name: "AddedDate",
                table: "MessHaveMembers",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "UpdateDate",
                table: "Messes",
                newName: "UpdateAt");

            migrationBuilder.RenameColumn(
                name: "AddedDate",
                table: "Messes",
                newName: "CreatedAt");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UpdateAt",
                table: "Persons",
                newName: "UpdateDate");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "Persons",
                newName: "AddedDate");

            migrationBuilder.RenameColumn(
                name: "UpdateAt",
                table: "MessHaveMembers",
                newName: "UpdateDate");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "MessHaveMembers",
                newName: "AddedDate");

            migrationBuilder.RenameColumn(
                name: "UpdateAt",
                table: "Messes",
                newName: "UpdateDate");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "Messes",
                newName: "AddedDate");
        }
    }
}
