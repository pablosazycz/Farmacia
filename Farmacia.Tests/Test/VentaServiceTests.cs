using Farmacia.Data;
using Farmacia.Interfaces;
using Farmacia.Models;
using Farmacia.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Moq;

namespace Farmacia.Tests.Test
{
    public class VentaServiceTests
    {
        [Fact]
        public async Task CrearVentaAsync_CalculaTotalCorrectamente()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDb")
                .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
                .Options;
            var context = new ApplicationDbContext(options);

            context.Productos.AddRange(
                new Producto { Id = 1, DrogaId = 1, NombreComercial = "Producto1", PrecioUnitario = 100, CantidadPresentacion = 1, FormaFarmaceutica = "Comprimido", TipoPresentacion = TipoPresentacionEnum.Caja, Activo = true },
                new Producto { Id = 2, DrogaId = 2, NombreComercial = "Producto2", PrecioUnitario = 50, CantidadPresentacion = 1, FormaFarmaceutica = "Comprimido", TipoPresentacion = TipoPresentacionEnum.Caja, Activo = true }
            );
            context.Drogas.AddRange(
                new Droga { Id = 1, Nombre = "Droga1", Concentracion = "500mg", Stock = 100 },
                new Droga { Id = 2, Nombre = "Droga2", Concentracion = "250mg", Stock = 100 }
            );
            // Agrega lotes con stock suficiente
            context.Lotes.AddRange(
                new Lote { CodigoLote = "L1", ProductoId = 1, Cantidad = 10, FechaVencimiento = DateTime.Now.AddMonths(6) },
                new Lote { CodigoLote = "L2", ProductoId = 2, Cantidad = 10, FechaVencimiento = DateTime.Now.AddMonths(6) }
            );
            await context.SaveChangesAsync();

            var mockMov = new Mock<IMovimientoStockService>();
            var mockCliente = new Mock<IClienteService>();
            var mockLote = new Mock<ILoteService>();
            var service = new VentaService(context, mockMov.Object, mockCliente.Object, mockLote.Object);

            var venta = new Venta
            {
                ClienteId = 1,
                UsuarioId = "usuario1",
                DetallesVenta = new List<DetalleVenta>
                {
                    new DetalleVenta { Cantidad = 2, PrecioUnitario = 100, Subtotal = 200, ProductoId = 1, CodigoLote = "L1" },
                    new DetalleVenta { Cantidad = 1, PrecioUnitario = 50, Subtotal = 50, ProductoId = 2, CodigoLote = "L2" }
                }
            };

            var resultado = await service.CrearVentaAsync(venta);

            Assert.Equal(250, resultado.Total);
            Assert.Equal(2, resultado.DetallesVenta.Count);
        }
    }
}