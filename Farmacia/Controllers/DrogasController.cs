using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Farmacia.Data;
using Farmacia.Models;
using Farmacia.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace Farmacia.Controllers

{
    [Authorize]
    public class DrogasController : Controller
    {
        private readonly IDrogaService _drogaService;
        private readonly IProductoService _productoService;

        public DrogasController(IDrogaService drogaService, IProductoService productoService)
        {
            _drogaService = drogaService;
            _productoService = productoService;
        }
        // GET: Drogas
        public async Task<IActionResult> Index()
        {
            return View(await _drogaService.ObtenerDrogasAsync());
        }

        // GET: Drogas/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Droga droga = await _drogaService.ObtenerDrogaPorIdAsync((int)id);
            if (droga == null)
            {
                return NotFound();
            }

            return View(droga);
        }

        // GET: Drogas/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Drogas/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Droga droga)
        {
            if (ModelState.IsValid)
            {
                await _drogaService.CrearDrogaAsync(droga);
                return RedirectToAction(nameof(Index));
            }
            return View(droga);
        }

        // GET: Drogas/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Droga droga = await _drogaService.ObtenerDrogaPorIdAsync((int)id);
            if (droga == null)
                return NotFound();

            return View(droga);
        }

        // POST: Drogas/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Droga droga)
        {
            if (id != droga.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                await _drogaService.EditarDrogaAsync(droga);
                return RedirectToAction(nameof(Index));
            }

            return View(droga);
        }

        // GET: Drogas/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Droga droga = await _drogaService.ObtenerDrogaPorIdAsync((int)id);
            if (droga == null)
                return NotFound();

            return View(droga);
        }

        // POST: Drogas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            Droga droga = await _drogaService.ObtenerDrogaPorIdAsync(id);
            if (droga == null)
                return NotFound();

            await _drogaService.EliminarDrogaAsync(droga);
            return RedirectToAction(nameof(Index));
        }

        private bool DrogaExists(int id)
        {
            return (_drogaService.ObtenerDrogasAsync().Result.Any(e => e.Id == id));
        }

        public async Task<IActionResult> ObtenerProductosPorDroga(int id)
        {
            List<Producto> productos = await _productoService.ObtenerProductosPorDrogaAsync(id);
            Droga droga = await _drogaService.ObtenerDrogaPorIdAsync(id);
            ViewData["Droga"] = droga.NombreCompleto;
            if (productos == null)
            {
                return NotFound();
            }
            return View(productos);
        }
    }
}
