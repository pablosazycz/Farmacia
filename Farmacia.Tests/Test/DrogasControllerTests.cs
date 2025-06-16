using Xunit;
using Moq;
using Farmacia.Controllers;
using Farmacia.Interfaces;
using Farmacia.Models;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Farmacia.Tests.Test
{
    public class DrogasControllerTests
    {
        [Fact]
        public async Task Create_Post_ValidModel_RedirectsToIndex()
        {
            // Arrange
            var mockService = new Mock<IDrogaService>();
            mockService.Setup(s => s.CrearDrogaAsync(It.IsAny<Droga>())).ReturnsAsync(new Droga());
            var controller = new DrogasController(mockService.Object, Mock.Of<IProductoService>());
            var droga = new Droga { Nombre = "Ibuprofeno", Concentracion = "400mg" };

            // Act
            var result = await controller.Create(droga);

            // Assert
            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirect.ActionName);
        }
    }
}