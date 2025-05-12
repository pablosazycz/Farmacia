using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Farmacia.Data;
using Farmacia.Models;
using Farmacia.Services;
using Farmacia.Interfaces;

namespace Farmacia.Controllers
{
    public class ProductosController : Controller
    {

        private readonly IProductoService _productoService;
        private readonly IDrogaService _drogaService;

        public ProductosController(IProductoService productoService, IDrogaService drogaService)
        {
            _productoService = productoService;
            _drogaService = drogaService;
        }


        // GET: Productos
        public async Task<IActionResult> Index()
        {
            List<Producto> productos = await _productoService.ObtenerProductosAsync();
            return View(productos);
        }

        // GET: Productos/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Producto producto = await _productoService.ObtenerProductoPorIdAsync(id);
            if (producto == null)
            {
                return NotFound();
            }

            return View(producto);
        }

        // GET: Productos/Create
        public async Task<IActionResult>  Create()
        {
            List<Droga> drogas = await _drogaService.ObtenerDrogasActivasAsync();
            ViewBag.Drogas = new SelectList(drogas, "Id", "NombreCompleto");

            return View();
        }

        // POST: Productos/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Producto producto)
        {
            if (ModelState.IsValid)
            {
                await _productoService.CrearProductoAsync(producto);
                return RedirectToAction(nameof(Index));
            }

            List<Droga> drogas = await _drogaService.ObtenerDrogasActivasAsync();
            ViewBag.Drogas = new SelectList(drogas, "Id", "NombreCompleto", producto.DrogaId);

            return View(producto);
        }

        // GET: Productos/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            Producto producto = await _productoService.ObtenerProductoPorIdAsync(id);
            if (producto == null) return NotFound();

            List<Droga> drogas = await _drogaService.ObtenerDrogasActivasAsync();
            ViewBag.Drogas = new SelectList(drogas, "Id", "NombreCompleto", producto.DrogaId);

            return View(producto);
        }

        // POST: Productos/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Producto producto)
        {
            if (id != producto.Id) return NotFound();

            if (ModelState.IsValid)
            {
                await _productoService.EditarProductoAsync(producto);
                return RedirectToAction(nameof(Index));
            }

            List<Droga> drogas = await _drogaService.ObtenerDrogasActivasAsync();
            ViewBag.Drogas = new SelectList(drogas, "Id", "NombreCompleto", producto.DrogaId);

            return View(producto);
        }

        // GET: Productos/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            Producto producto = await _productoService.ObtenerProductoPorIdAsync(id);
            if (producto == null) return NotFound();

            return View(producto);
        }

        // POST: Productos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            Producto producto = await _productoService.ObtenerProductoPorIdAsync(id);
            if (producto == null) return NotFound();

            await _productoService.EliminarProductoAsync(producto);
            return RedirectToAction(nameof(Index));
        }

        
    }
}
