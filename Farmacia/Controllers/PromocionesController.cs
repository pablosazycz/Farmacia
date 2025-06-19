using Farmacia.Interfaces;
using Farmacia.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Farmacia.Controllers
{
    [Authorize]
    public class PromocionesController : Controller
    {
        private readonly IPromocionService _promocionService;

        public PromocionesController(IPromocionService promocionService)
        {
            _promocionService = promocionService;
        }

        public async Task<IActionResult> Index()
        {
            var promociones = await _promocionService.ObtenerPromocionesAsync();
            return View(promociones);
        }

        public async Task<IActionResult> Details(int id)
        {
            var promocion = await _promocionService.ObtenerPromocionPorIdAsync(id);
            if (promocion == null) return NotFound();
            return View(promocion);
        }

        [Authorize(Roles = "Administrador")]
        public IActionResult Create()
        {
            return View();
        }
        [Authorize(Roles = "Administrador")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Promocion promocion)
        {
            if (ModelState.IsValid)
            {
                // Normaliza el descuento: si es mayor a 1, lo convierte a fracción
                if (promocion.Descuento > 1)
                    promocion.Descuento = promocion.Descuento / 100m;

                await _promocionService.CrearPromocionAsync(promocion);
                return RedirectToAction(nameof(Index));
            }
            return View(promocion);
        }
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Edit(int id)
        {
            var promocion = await _promocionService.ObtenerPromocionPorIdAsync(id);
            if (promocion == null) return NotFound();
            return View(promocion);
        }
        [Authorize(Roles = "Administrador")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Promocion promocion)
        {
            if (id != promocion.Id) return BadRequest();
            if (ModelState.IsValid)
            {
                // Normaliza el descuento: si es mayor a 1, lo convierte a fracción
                if (promocion.Descuento > 1)
                    promocion.Descuento = promocion.Descuento / 100m;

                await _promocionService.EditarPromocionAsync(promocion);
                return RedirectToAction(nameof(Index));
            }
            return View(promocion);
        }
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Delete(int id)
        {
            var promocion = await _promocionService.ObtenerPromocionPorIdAsync(id);
            if (promocion == null) return NotFound();
            return View(promocion);
        }
        [Authorize(Roles = "Administrador")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _promocionService.EliminarPromocionAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}