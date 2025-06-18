using Farmacia.Controllers;
using Farmacia.Interfaces;
using Farmacia.Models;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Farmacia.Tests.Test
{
    public class ProductosControllerTests
    {
        [Fact]
        public async Task Create_Post_ValidModel_RedirectsToIndex()
        {
            // Arrange
            var mockProductoService = new Mock<IProductoService>();
            var mockDrogaService = new Mock<IDrogaService>();
            mockProductoService.Setup(s => s.CrearProductoAsync(It.IsAny<Producto>())).ReturnsAsync(new Producto());
            mockDrogaService.Setup(s => s.ObtenerDrogasActivasAsync()).ReturnsAsync(new List<Droga>());
            var controller = new ProductosController(mockProductoService.Object, mockDrogaService.Object);
            var producto = new Producto { NombreComercial = "Ibuprofeno", FormaFarmaceutica = "Comprimido", TipoPresentacion = TipoPresentacionEnum.Caja, DrogaId = 1 };

            // Act
            var result = await controller.Create(producto);

            // Assert
            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirect.ActionName);
        }
    }
}