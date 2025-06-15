using Farmacia.Interfaces;
using Farmacia.Models;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Farmacia.Controllers
{
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

        public IActionResult Create()
        {
            return View();
        }

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

        public async Task<IActionResult> Edit(int id)
        {
            var promocion = await _promocionService.ObtenerPromocionPorIdAsync(id);
            if (promocion == null) return NotFound();
            return View(promocion);
        }

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

        public async Task<IActionResult> Delete(int id)
        {
            var promocion = await _promocionService.ObtenerPromocionPorIdAsync(id);
            if (promocion == null) return NotFound();
            return View(promocion);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _promocionService.EliminarPromocionAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}