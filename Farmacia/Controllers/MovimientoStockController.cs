using Farmacia.Data;
using Farmacia.Interfaces;
using Farmacia.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Identity;


namespace Farmacia.Controllers
{
    public class MovimientoStockController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IDrogaService _drogaService;
        private readonly IProductoService _productoService;
        private readonly ILoteService _loteService;
        private readonly IMovimientoStockService _movimientoStockService;
        private readonly UserManager<IdentityUser> _userManager;



        public MovimientoStockController(
            ApplicationDbContext context,
            IDrogaService drogaService,
            IProductoService productoService,
            ILoteService loteService,
            IMovimientoStockService movimientoStockService,
            UserManager<IdentityUser> userManager
            )
        {
            _context = context;
            _drogaService = drogaService;
            _productoService = productoService;
            _loteService = loteService;
            _movimientoStockService = movimientoStockService;
            _userManager = userManager;
        }

        // GET: MovimientoStock
        public async Task<IActionResult> Index()
        {
            var movimientos = await _movimientoStockService.ObtenerTodosAsync(); // o como lo hayas llamado
            return View(movimientos);
        }

        // GET: MovimientoStock/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: MovimientoStock/CrearCompra
        public async Task<IActionResult> CrearCompra()
        {
            IList<Droga> drogas = await _drogaService.ObtenerDrogasAsync();


            ViewBag.Drogas = new SelectList(drogas, "Id", "NombreCompleto");

            return View();
        }

        public async Task<IActionResult> ObtenerProductosPorDroga(int drogaId)
        {
            var productos = await _productoService.ObtenerProductosPorDrogaAsync(drogaId);
            if (productos == null || !productos.Any())
            {
                return Json(new { success = false, message = "No se encontraron productos para esta droga." });
            }

            return Json(new { success = true, productos = productos });
        }

        // POST: MovimientoStock/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CrearCompra(MovimientoStock movimiento)
        {
            movimiento.UsuarioId = _userManager.GetUserId(User);

            if (ModelState.IsValid)
            {
                movimiento.Fecha = DateTime.Now;
                movimiento.TipoMovimiento = TipoMovimiento.Compra;

                await _movimientoStockService.CrearMovimientoAsync(movimiento);
                return RedirectToAction("Index");
            }

            ViewBag.Drogas = new SelectList(await _drogaService.ObtenerDrogasAsync(), "Id", "NombreCompleto");
            return View(movimiento);
        }


        public async Task<IActionResult> CrearVenta()
        {
            IList<Droga> drogas = await _drogaService.ObtenerDrogasAsync();


            ViewBag.Drogas = new SelectList(drogas, "Id", "NombreCompleto");

            return View();
        }

     
        // POST: MovimientoStock/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CrearVenta(MovimientoStock movimiento)
        {
            movimiento.UsuarioId = _userManager.GetUserId(User);

            if (ModelState.IsValid)
            {
                movimiento.Fecha = DateTime.Now;
                movimiento.TipoMovimiento = TipoMovimiento.Venta;

                await _movimientoStockService.CrearMovimientoAsync(movimiento);
                return RedirectToAction("Index");
            }

            ViewBag.Drogas = new SelectList(await _drogaService.ObtenerDrogasAsync(), "Id", "NombreCompleto");
            return View(movimiento);
        }


        // GET: MovimientoStock/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: MovimientoStock/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: MovimientoStock/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: MovimientoStock/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        [HttpGet]
        public async Task<IActionResult> AltaRotacion()
        {
            var desde = DateTime.Today.AddMonths(-1);
            var hasta = DateTime.Today;
            var productosAltaRotacion = await _productoService.ObtenerProductosAltaRotacionConStockAsync(desde, hasta, 10);
            return View(productosAltaRotacion);
        }

        public async Task<IActionResult> SugerenciasReposicion()
        {
            var productosReposicion = await _productoService.ObtenerProductosParaReposicionAsync();
            return View(productosReposicion);
        }

        public async Task<IActionResult> SugerenciasPromocion(int? diasAVencer, int? stockAlto)
        {
            int dias = diasAVencer ?? 30;
            int stock = stockAlto ?? 10;
            var productosPromocion = await _productoService.ObtenerProductosParaPromocionAsync(dias, stock);
            ViewBag.DiasAVencer = dias;
            ViewBag.StockAlto = stock;
            return View(productosPromocion);
        }

        public async Task<IActionResult> ReporteVentasPorProducto(DateTime? desde, DateTime? hasta)
        {
            var fechaDesde = desde ?? DateTime.Today.AddMonths(-1);
            var fechaHasta = hasta ?? DateTime.Today.AddDays(1);
            var reporte = await _productoService.ObtenerReporteVentasPorProductoAsync(fechaDesde, fechaHasta);
            ViewBag.FechaDesde = fechaDesde.ToString("yyyy-MM-dd");
            ViewBag.FechaHasta = fechaHasta.ToString("yyyy-MM-dd");
            return View(reporte);
        }

    }
}
