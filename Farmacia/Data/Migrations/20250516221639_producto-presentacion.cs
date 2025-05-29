﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Farmacia.Data.Migrations
{
    /// <inheritdoc />
    public partial class productopresentacion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MovimientosStock_AspNetUsers_UsuarioId",
                table: "MovimientosStock");

            migrationBuilder.AddColumn<int>(
                name: "TipoPresentacion",
                table: "Productos",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<string>(
                name: "UsuarioId",
                table: "MovimientosStock",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddForeignKey(
                name: "FK_MovimientosStock_AspNetUsers_UsuarioId",
                table: "MovimientosStock",
                column: "UsuarioId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MovimientosStock_AspNetUsers_UsuarioId",
                table: "MovimientosStock");

            migrationBuilder.DropColumn(
                name: "TipoPresentacion",
                table: "Productos");

            migrationBuilder.AlterColumn<string>(
                name: "UsuarioId",
                table: "MovimientosStock",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_MovimientosStock_AspNetUsers_UsuarioId",
                table: "MovimientosStock",
                column: "UsuarioId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
