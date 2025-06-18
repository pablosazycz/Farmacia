using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Farmacia.Data.Migrations
{
    /// <inheritdoc />
    public partial class PrecioCompra : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "PrecioCompra",
                table: "Lotes",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PrecioCompra",
                table: "Lotes");
        }
    }
}
