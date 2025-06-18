using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Farmacia.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddClientePunto : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ClientePuntos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ClienteId = table.Column<int>(type: "int", nullable: false),
                    Puntos = table.Column<int>(type: "int", nullable: false),
                    Fecha = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Motivo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PromocionId = table.Column<int>(type: "int", nullable: true),
                    VentaId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClientePuntos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ClientePuntos_Clientes_ClienteId",
                        column: x => x.ClienteId,
                        principalTable: "Clientes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ClientePuntos_Promociones_PromocionId",
                        column: x => x.PromocionId,
                        principalTable: "Promociones",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ClientePuntos_Ventas_VentaId",
                        column: x => x.VentaId,
                        principalTable: "Ventas",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_ClientePuntos_ClienteId",
                table: "ClientePuntos",
                column: "ClienteId");

            migrationBuilder.CreateIndex(
                name: "IX_ClientePuntos_PromocionId",
                table: "ClientePuntos",
                column: "PromocionId");

            migrationBuilder.CreateIndex(
                name: "IX_ClientePuntos_VentaId",
                table: "ClientePuntos",
                column: "VentaId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ClientePuntos");
        }
    }
}
