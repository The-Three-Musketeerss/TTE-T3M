using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TTE.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class JobsUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "Jobs",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "ItemName",
                table: "Jobs",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "Jobs");

            migrationBuilder.DropColumn(
                name: "ItemName",
                table: "Jobs");
        }
    }
}
