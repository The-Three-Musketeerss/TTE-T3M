using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TTE.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddWishlistUniqueConstraintRaw : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Usa SQL directo para evitar conflictos con claves foráneas en MySQL
            migrationBuilder.Sql(@"
                ALTER TABLE `Wishlists`
                ADD CONSTRAINT `UX_Wishlists_UserId_ProductId`
                UNIQUE (`UserId`, `ProductId`);
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Elimina la restricción única si se revierte la migración
            migrationBuilder.Sql(@"
                ALTER TABLE `Wishlists`
                DROP INDEX `UX_Wishlists_UserId_ProductId`;
            ");
        }
    }
}
