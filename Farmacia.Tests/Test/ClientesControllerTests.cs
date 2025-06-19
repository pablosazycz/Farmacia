using Farmacia.Controllers;
using Farmacia.Interfaces;
using Farmacia.Models;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Farmacia.Tests.Test
{
    public class ClientesControllerTests
    {
        // Crear Cliente
        [Fact]
        public async Task CrearClienteYRedirigir()
        {
            // Arrange
            var mockService = new Mock<IClienteService>();
            mockService.Setup(s => s.CrearClienteAsync(It.IsAny<Cliente>())).Returns(Task.CompletedTask);
            var controller = new ClientesController(mockService.Object);
            var cliente = new Cliente { Nombre = "Juan", Apellido = "Pérez", Dni = "12345678" };

            // Act
            var result = await controller.Create(cliente);

            // Assert
            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirect.ActionName);
        }

        // Editar cliente 
        [Fact]
        public async Task EditarClienteYRedirigir()
        {
            // Arrange
            var mockClienteService = new Mock<IClienteService>();
            var cliente = new Cliente { Id = 1, Nombre = "Juan", Apellido = "Pérez", Dni = "12345678" };
            mockClienteService.Setup(s => s.EditarClienteAsync(cliente)).Returns(Task.CompletedTask);
            var controller = new ClientesController(mockClienteService.Object);

            // Act
            var result = await controller.Edit(1, cliente);

            // Assert
            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirect.ActionName);
            mockClienteService.Verify(s => s.EditarClienteAsync(cliente), Times.Once);
        }

        // Eliminar cliente existente
        [Fact]
        public async Task EliminarClienteYRedirigir()
        {
            // Arrange
            var mockClienteService = new Mock<IClienteService>();
            var cliente = new Cliente { Id = 2, Nombre = "Ana", Apellido = "García", Dni = "87654321" };
            mockClienteService.Setup(s => s.EliminarClienteAsync(2)).Returns(Task.CompletedTask);
            var controller = new ClientesController(mockClienteService.Object);

            // Act
            var result = await controller.DeleteConfirmed(2);

            // Assert
            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirect.ActionName);
            mockClienteService.Verify(s => s.EliminarClienteAsync(2), Times.Once);
        }
    }
}