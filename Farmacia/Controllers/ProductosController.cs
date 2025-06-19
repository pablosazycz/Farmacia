using Farmacia.Interfaces;
using Farmacia.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Farmacia.Controllers
{
    [Authorize]
    public class ProductosController : Controller
    {

        private readonly IProductoService _productoService;
        private readonly IDrogaService _drogaService;

        public ProductosController(IProductoService productoService, IDrogaService drogaService)
        {
            _productoService = productoService;
            _drogaService = drogaService;
        }

        [HttpGet]
        public async Task<IActionResult> Buscar(string term)
        {
            var productos = await _productoService.BuscarProductosAsync(term);

            var resultado = productos.Select(p => new
            {
                id = p.Id,
                text = p.NombreComercial,
                descripcion = $" [{p.TipoPresentacion}]"
                + (p.CantidadPresentacion > 0 ? $" [{p.CantidadPresentacion}]" : "")
               + (p.Droga != null && !string.IsNullOrEmpty(p.Droga.Nombre) ? $" [{p.Droga.Nombre}]" : "")
               + (p.Droga != null && !string.IsNullOrEmpty(p.Droga.Concentracion) ? $" [{p.Droga.Concentracion}]" : "")
               + $" [${p.PrecioUnitario:F2}]",
                precioUnitario = p.PrecioUnitario
            });


            return Json(new { results = resultado });
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
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Create()
        {
            List<Droga> drogas = await _drogaService.ObtenerDrogasActivasAsync();
            ViewBag.Drogas = new SelectList(drogas, "Id", "NombreCompleto");

            return View();
        }

        // POST: Productos/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Administrador")]
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
        [Authorize(Roles = "Administrador")]
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
        [Authorize(Roles = "Administrador")]
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
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            Producto producto = await _productoService.ObtenerProductoPorIdAsync(id);
            if (producto == null) return NotFound();

            return View(producto);
        }

        // POST: Productos/Delete/5
        [Authorize(Roles = "Administrador")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            Producto producto = await _productoService.ObtenerProductoPorIdAsync(id);
            if (producto == null) return NotFound();

            await _productoService.EliminarProductoAsync(producto);
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> ProductosPorDroga(int id)
        {
            try
            {
                var productosRelacionados = await _productoService.ObtenerProductosPorDrogaOrdenadosPorVencimientoAsync(id);
                return PartialView("_ProductosSugeridosPorDroga", productosRelacionados);
            }
            catch (Exception ex)
            {
                return Content("Error: " + ex.Message);
            }
        }
    }
}
