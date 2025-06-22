using Farmacia.Controllers;
using Farmacia.Interfaces;
using Farmacia.Models;
using Farmacia.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Farmacia.Tests.Test
{
    public class ProductosControllerTests
    {
        // Crear producto
        [Fact]
        public async Task CrearProductoYRedirigir()
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

        // Buscar producto por Nombre, devuelve activos y coincidentes

        [Fact]
        public async Task ObtenerProductosPorNombre()
        {
            // Arrange
            var mockProductoService = new Mock<IProductoService>();
            var mockDrogaService = new Mock<IDrogaService>();
            var productos = new List<Producto>
    {
        new Producto { NombreComercial = "Paracetamol", Activo = true, DrogaId = 1, Droga = new Droga { Id = 1, Nombre = "Paracetamol", Concentracion = "500mg", Activo = true } },
        new Producto { NombreComercial = "Ibuprofeno", Activo = true, DrogaId = 1, Droga = new Droga { Id = 1, Nombre = "Paracetamol", Concentracion = "500mg", Activo = true } },
        new Producto { NombreComercial = "Paracetamol Forte", Activo = false, DrogaId = 1, Droga = new Droga { Id = 1, Nombre = "Paracetamol", Concentracion = "500mg", Activo = true } }
    };
            mockProductoService.Setup(s => s.ObtenerProductosPorNombreAsync("Paracetamol"))
                .ReturnsAsync(productos.Where(p => p.NombreComercial.Contains("Paracetamol") && p.Activo).ToList());

            var controller = new ProductosController(mockProductoService.Object, mockDrogaService.Object);

            // Act
            var resultado = await mockProductoService.Object.ObtenerProductosPorNombreAsync("Paracetamol");

            // Assert
            Assert.Single(resultado);
            Assert.Equal("Paracetamol", resultado.First().NombreComercial);
            Assert.True(resultado.All(p => p.Activo));
            Assert.All(resultado, p => Assert.NotNull(p.Droga));
        }

        // Ver detalles de producto
        [Fact]
        public async Task DetallesProducto_ConVista()
        {
            // Arrange
            var mockProductoService = new Mock<IProductoService>();
            var mockDrogaService = new Mock<IDrogaService>();
            var producto = new Producto { Id = 5, NombreComercial = "Ibuprofeno" };
            mockProductoService.Setup(s => s.ObtenerProductoPorIdAsync(5)).ReturnsAsync(producto);
            var controller = new ProductosController(mockProductoService.Object, mockDrogaService.Object);

            // Act
            var result = await controller.Details(5);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal(producto, viewResult.Model);
        }

        // editar producto
        [Fact]
        public async Task EditarProductoYRedirigir()
        {
            // Arrange
            var mockProductoService = new Mock<IProductoService>();
            var mockDrogaService = new Mock<IDrogaService>();
            var producto = new Producto { Id = 1, NombreComercial = "Ibuprofeno", DrogaId = 2 };
            mockProductoService.Setup(s => s.EditarProductoAsync(producto)).ReturnsAsync(producto);

            var controller = new ProductosController(mockProductoService.Object, mockDrogaService.Object);

            // Act
            var result = await controller.Edit(1, producto);

            // Assert
            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirect.ActionName);
            mockProductoService.Verify(s => s.EditarProductoAsync(producto), Times.Once);
        }

        // Eliminar producto
        [Fact]
        public async Task EliminarProductoYRedrigir()
        {
            // Arrange
            var mockProductoService = new Mock<IProductoService>();
            var mockDrogaService = new Mock<IDrogaService>();
            var producto = new Producto { Id = 1, NombreComercial = "Ibuprofeno" };
            mockProductoService.Setup(s => s.ObtenerProductoPorIdAsync(1)).ReturnsAsync(producto);
            mockProductoService.Setup(s => s.EliminarProductoAsync(producto)).ReturnsAsync(producto);

            var controller = new ProductosController(mockProductoService.Object, mockDrogaService.Object);

            // Act
            var result = await controller.DeleteConfirmed(1);

            // Assert
            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirect.ActionName);
            mockProductoService.Verify(s => s.EliminarProductoAsync(producto), Times.Once);
        }
    }
}