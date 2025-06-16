using Xunit;
using Moq;
using Farmacia.Controllers;
using Farmacia.Interfaces;
using Farmacia.Models;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Farmacia.Tests.Test
{
    public class ClientesControllerTests
    {
        [Fact]
        public async Task Create_Post_ValidModel_RedirectsToIndex()
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
    }
}