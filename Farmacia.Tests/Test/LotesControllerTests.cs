using Farmacia.Controllers;
using Farmacia.Data;
using Farmacia.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Xunit;

namespace Farmacia.Tests.Test
{
    public class LotesControllerTests
    {
        // Db Context
        private static async Task<ApplicationDbContext> GetContextAsync(string dbName)
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(dbName)
                .Options;
            var context = new ApplicationDbContext(options);
            if (!await context.Productos.AnyAsync())
            {
                context.Productos.Add(new Producto { Id = 1, NombreComercial = "Producto", FormaFarmaceutica = "Comprimido", TipoPresentacion = TipoPresentacionEnum.Caja, DrogaId = 1 });
                await context.SaveChangesAsync();
            }
            return context;
        }

        // Crear Lote
        [Fact]
        public async Task CrearLoteYRedirigir()
        {
            await using var context = await GetContextAsync("LotesTestDb");
            var controller = new LotesController(context);
            var lote = new Lote { CodigoLote = "L1", FechaVencimiento = System.DateTime.Now.AddYears(1), Cantidad = 10, ProductoId = 1 };

            var result = await controller.Create(lote);

            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirect.ActionName);
        }

        // Editar Lote
        [Fact]
        public async Task EditarLoteYRedirigir()
        {
            await using var context = await GetContextAsync("LotesEditTestDb");
            var lote = new Lote { Id = 1, CodigoLote = "L2", FechaVencimiento = System.DateTime.Now.AddMonths(6), Cantidad = 20, ProductoId = 1 };
            context.Lotes.Add(lote);
            await context.SaveChangesAsync();

            var controller = new LotesController(context);
            lote.Cantidad = 30;

            var result = await controller.Edit(1, lote);

            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirect.ActionName);

            var loteEditado = await context.Lotes.FindAsync(1);
            Assert.Equal(30, loteEditado.Cantidad);
        }

        [Fact]
        public async Task EliminarLoteYRedirigir()
        {
            await using var context = await GetContextAsync("LotesDeleteTestDb");
            var lote = new Lote { Id = 2, CodigoLote = "L3", FechaVencimiento = System.DateTime.Now.AddMonths(12), Cantidad = 50, ProductoId = 1 };
            context.Lotes.Add(lote);
            await context.SaveChangesAsync();

            var controller = new LotesController(context);

            var result = await controller.DeleteConfirmed(2);

            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirect.ActionName);

            var loteEliminado = await context.Lotes.FindAsync(2);
            Assert.Null(loteEliminado);
        }
    }
}