using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Farmacia.Data.Migrations
{
    /// <inheritdoc />
    public partial class addVentaIdMovimientoStock : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "VentaId",
                table: "MovimientosStock",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Subtotal",
                table: "DetallesVenta",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateIndex(
                name: "IX_MovimientosStock_VentaId",
                table: "MovimientosStock",
                column: "VentaId");

            migrationBuilder.AddForeignKey(
                name: "FK_MovimientosStock_Ventas_VentaId",
                table: "MovimientosStock",
                column: "VentaId",
                principalTable: "Ventas",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MovimientosStock_Ventas_VentaId",
                table: "MovimientosStock");

            migrationBuilder.DropIndex(
                name: "IX_MovimientosStock_VentaId",
                table: "MovimientosStock");

            migrationBuilder.DropColumn(
                name: "VentaId",
                table: "MovimientosStock");

            migrationBuilder.DropColumn(
                name: "Subtotal",
                table: "DetallesVenta");
        }
    }
}
