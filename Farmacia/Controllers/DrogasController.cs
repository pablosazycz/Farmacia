using Farmacia.Interfaces;
using Farmacia.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
        [Authorize(Roles = "Administrador")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Drogas/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Administrador")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Droga droga)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _drogaService.CrearDrogaAsync(droga);
                    return RedirectToAction(nameof(Index));
                }
                catch (InvalidOperationException ex)
                {
                    // Agrega el mensaje de error a ModelState para mostrarlo en la vista
                    ModelState.AddModelError(string.Empty, ex.Message);
                }
                catch (Exception)
                {
                    ModelState.AddModelError(string.Empty, "Ocurrió un error inesperado.");
                }
            }
            return View(droga);
        }

        // GET: Drogas/Edit/5
        [Authorize(Roles = "Administrador")]
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
        [Authorize(Roles = "Administrador")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Droga droga)
        {
            if (id != droga.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    await _drogaService.EditarDrogaAsync(droga);
                    return RedirectToAction(nameof(Index));
                }
                catch (InvalidOperationException ex)
                {
                    // Agrega el mensaje de error a ModelState para mostrarlo en la vista
                    ModelState.AddModelError(string.Empty, ex.Message);
                }
                catch (Exception)
                {
                    ModelState.AddModelError(string.Empty, "Ocurrió un error inesperado.");
                }
            }

            return View(droga);
        }

        // GET: Drogas/Delete/5
        [Authorize(Roles = "Administrador")]
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
        [Authorize(Roles = "Administrador")]
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

        [HttpGet]
        public async Task<IActionResult> Buscar(string term)
        {
            var drogas = await _drogaService.BuscarDrogasAsync(term);
            var resultado = drogas.Select(d => new { id = d.Id, text = d.NombreCompleto });
            return Json(new { results = resultado });
        }
    }
}
