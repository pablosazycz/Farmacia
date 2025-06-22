using Farmacia.Controllers;
using Farmacia.Interfaces;
using Farmacia.Models;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Farmacia.Tests.Test
{
    public class DrogasControllerTests
    {
        // Crear droga
        [Fact]
        public async Task CrearDrogaYRedirigir()
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

        // Ver detalle de la Droga
        [Fact]
        public async Task DetallesDeDroga_ConVista()
        {
            var mockDrogaService = new Mock<IDrogaService>();
            var droga = new Droga { Id = 1, Nombre = "Paracetamol", Concentracion = "500mg" };
            mockDrogaService.Setup(s => s.ObtenerDrogaPorIdAsync(1)).ReturnsAsync(droga);
            var controller = new DrogasController(mockDrogaService.Object, Mock.Of<IProductoService>());

            var result = await controller.Details(1);

            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal(droga, viewResult.Model);
        }

        // Editar droga 
        [Fact]
        public async Task EditarDrogaYRedirigir()
        {
            var mockDrogaService = new Mock<IDrogaService>();
            var droga = new Droga { Id = 3, Nombre = "Amoxicilina", Concentracion = "250mg" };
            mockDrogaService.Setup(s => s.EditarDrogaAsync(droga)).ReturnsAsync(droga);
            var controller = new DrogasController(mockDrogaService.Object, Mock.Of<IProductoService>());

            var result = await controller.Edit(3, droga);

            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirect.ActionName);
            mockDrogaService.Verify(s => s.EditarDrogaAsync(droga), Times.Once);
        }


        // Eliminar droga 
        [Fact]
        public async Task EliminarDrogaYRedirigir()
        {
            var mockDrogaService = new Mock<IDrogaService>();
            var droga = new Droga { Id = 5, Nombre = "Metformina", Concentracion = "850mg" };
            mockDrogaService.Setup(s => s.ObtenerDrogaPorIdAsync(5)).ReturnsAsync(droga);
            mockDrogaService.Setup(s => s.EliminarDrogaAsync(droga)).ReturnsAsync(droga);
            var controller = new DrogasController(mockDrogaService.Object, Mock.Of<IProductoService>());

            var result = await controller.DeleteConfirmed(5);

            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirect.ActionName);
            mockDrogaService.Verify(s => s.EliminarDrogaAsync(droga), Times.Once);
        }


    }
}