using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SportsEvents.Web.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateEventImagesToUrl : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FilePath",
                table: "EventImages");

            migrationBuilder.AddColumn<string>(
                name: "ImageUrl",
                table: "EventImages",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageUrl",
                table: "EventImages");

            migrationBuilder.AddColumn<string>(
                name: "FilePath",
                table: "EventImages",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");
        }
    }
}
