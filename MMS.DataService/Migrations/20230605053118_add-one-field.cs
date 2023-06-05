using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MMS.DataService.Migrations
{
    /// <inheritdoc />
    public partial class addonefield : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsEnd",
                table: "Days",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsEnd",
                table: "Days");
        }
    }
}
