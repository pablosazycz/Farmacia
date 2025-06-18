using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Farmacia.Data.Migrations
{
    /// <inheritdoc />
    public partial class CambiosFinales : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Activo",
                table: "Productos",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaAlta",
                table: "Productos",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaBaja",
                table: "Productos",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "TipoMovimiento",
                table: "MovimientosStock",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Observaciones",
                table: "MovimientosStock",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CodigoLote",
                table: "MovimientosStock",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "LoteId",
                table: "MovimientosStock",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ProductoId",
                table: "MovimientosStock",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Activo",
                table: "Drogas",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaAlta",
                table: "Drogas",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaBaja",
                table: "Drogas",
                type: "datetime2",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_MovimientosStock_LoteId",
                table: "MovimientosStock",
                column: "LoteId");

            migrationBuilder.CreateIndex(
                name: "IX_MovimientosStock_ProductoId",
                table: "MovimientosStock",
                column: "ProductoId");

            migrationBuilder.AddForeignKey(
                name: "FK_MovimientosStock_Lotes_LoteId",
                table: "MovimientosStock",
                column: "LoteId",
                principalTable: "Lotes",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_MovimientosStock_Productos_ProductoId",
                table: "MovimientosStock",
                column: "ProductoId",
                principalTable: "Productos",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MovimientosStock_Lotes_LoteId",
                table: "MovimientosStock");

            migrationBuilder.DropForeignKey(
                name: "FK_MovimientosStock_Productos_ProductoId",
                table: "MovimientosStock");

            migrationBuilder.DropIndex(
                name: "IX_MovimientosStock_LoteId",
                table: "MovimientosStock");

            migrationBuilder.DropIndex(
                name: "IX_MovimientosStock_ProductoId",
                table: "MovimientosStock");

            migrationBuilder.DropColumn(
                name: "Activo",
                table: "Productos");

            migrationBuilder.DropColumn(
                name: "FechaAlta",
                table: "Productos");

            migrationBuilder.DropColumn(
                name: "FechaBaja",
                table: "Productos");

            migrationBuilder.DropColumn(
                name: "CodigoLote",
                table: "MovimientosStock");

            migrationBuilder.DropColumn(
                name: "LoteId",
                table: "MovimientosStock");

            migrationBuilder.DropColumn(
                name: "ProductoId",
                table: "MovimientosStock");

            migrationBuilder.DropColumn(
                name: "Activo",
                table: "Drogas");

            migrationBuilder.DropColumn(
                name: "FechaAlta",
                table: "Drogas");

            migrationBuilder.DropColumn(
                name: "FechaBaja",
                table: "Drogas");

            migrationBuilder.AlterColumn<string>(
                name: "TipoMovimiento",
                table: "MovimientosStock",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "Observaciones",
                table: "MovimientosStock",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true);
        }
    }
}
