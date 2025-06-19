using Farmacia.Controllers;
using Farmacia.Interfaces;
using Farmacia.Models;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Farmacia.Tests.Test
{
    public class PromocionesControllerTests
    {
        // Ver promociones
        [Fact]
        public async Task VerPromociones()
        {
            var mockService = new Mock<IPromocionService>();
            mockService.Setup(s => s.ObtenerPromocionesAsync())
                .ReturnsAsync(new List<Promocion> { new Promocion { Id = 1, Nombre = "Promo1" } });

            var controller = new PromocionesController(mockService.Object);

            var result = await controller.Index();

            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<List<Promocion>>(viewResult.Model);
            Assert.Single(model);
            Assert.Equal("Promo1", model[0].Nombre);
        }

        // Detalles promo existente
        [Fact]
        public async Task DetallesPromo_ConVista()
        {
            var promo = new Promocion { Id = 2, Nombre = "Promo2" };
            var mockService = new Mock<IPromocionService>();
            mockService.Setup(s => s.ObtenerPromocionPorIdAsync(2)).ReturnsAsync(promo);

            var controller = new PromocionesController(mockService.Object);

            var result = await controller.Details(2);

            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal(promo, viewResult.Model);
        }

        // Crear promoción
        [Fact]
        public async Task CrearPromoYRedirigir()
        {
            var mockService = new Mock<IPromocionService>();
            var controller = new PromocionesController(mockService.Object);
            var promo = new Promocion { Nombre = "Promo3", Descuento = 0.2m, Tipo = "Manual" };

            var result = await controller.Create(promo);

            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirect.ActionName);
            mockService.Verify(s => s.CrearPromocionAsync(It.IsAny<Promocion>()), Times.Once);
        }

        // Editar promoción
        [Fact]
        public async Task EditarPromoYRedirigir()
        {
            var mockService = new Mock<IPromocionService>();
            var controller = new PromocionesController(mockService.Object);
            var promo = new Promocion { Id = 4, Nombre = "Promo4", Descuento = 0.1m, Tipo = "Manual" };

            var result = await controller.Edit(4, promo);

            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirect.ActionName);
            mockService.Verify(s => s.EditarPromocionAsync(promo), Times.Once);
        }

        // Eliminar promoción
        [Fact]
        public async Task EliminarPromoYRedirigir()
        {
            var mockService = new Mock<IPromocionService>();
            var controller = new PromocionesController(mockService.Object);

            var result = await controller.DeleteConfirmed(5);

            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirect.ActionName);
            mockService.Verify(s => s.EliminarPromocionAsync(5), Times.Once);
        }
    }
}