using Xunit;
using Farmacia.Controllers;
using Farmacia.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Farmacia.Data;
using System.Threading.Tasks;

namespace Farmacia.Tests.Test
{
    public class LotesControllerTests
    {
        [Fact]
        public async Task Create_Post_ValidModel_RedirectsToIndex()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase("LotesTestDb")
                .Options;
            var context = new ApplicationDbContext(options);
            context.Productos.Add(new Producto { Id = 1, NombreComercial = "Ibuprofeno", FormaFarmaceutica = "Comprimido", TipoPresentacion = TipoPresentacionEnum.Caja, DrogaId = 1 });
            await context.SaveChangesAsync();

            var controller = new LotesController(context);
            var lote = new Lote { CodigoLote = "L1", FechaVencimiento = System.DateTime.Now.AddYears(1), Cantidad = 10, ProductoId = 1 };

            // Act
            var result = await controller.Create(lote);

            // Assert
            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirect.ActionName);
        }
    }
}