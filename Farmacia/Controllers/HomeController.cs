using Farmacia.Interfaces;
using Farmacia.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;

namespace Farmacia.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ILoteService _loteService;
        private readonly IProductoService _productoService;
        private readonly UserManager<IdentityUser> _UserManager;

        public HomeController(ILogger<HomeController> logger, ILoteService loteService, UserManager<IdentityUser> userManager, IProductoService productoService)
        {
            _logger = logger;
            _loteService = loteService;
            _UserManager = userManager;
            _productoService = productoService;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var lotesVencidos = await _loteService.ObtenerLotesVencidosAsync();

                var lotesProximos = await _loteService.ObtenerLotesProximosAVencerAsync(30);

                var productosStockCritico = await _productoService.ObtenerProductosConStockCriticoAsync();

                if (productosStockCritico.Any())
                    ViewBag.AlertaStock = "¡Atención! Hay productos con stock crítico.";
                if (lotesVencidos.Any())
                    ViewBag.MensajeVencidos = "¡Atención! Hay productos vencidos.";
                if (lotesProximos.Any())
                    ViewBag.MensajeProximos = "¡Aviso! Hay productos próximos a vencer.";


                return View();
            }
            catch (Exception ex)

            {
                return BadRequest(ex.Message);

            }
            //return View();

        }

        public async Task<IActionResult> ProxAVencer()
        {
            try
            {
                var lotes = await _loteService.ObtenerLotesProximosAVencerAsync(30); // Obtiene lotes que vencen en los próximos 30 días
                return View(lotes);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener lotes próximos a vencer");
                return View("Error", new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
            }
        }

        public async Task<IActionResult> Vencidos()
        {
            try
            {
                var lotes = await _loteService.ObtenerLotesVencidosAsync(); // Obtiene lotes que ya han vencido
                return View(lotes);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener lotes vencidos");
                return View("Error", new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
            }
        }

        [HttpPost]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> DarDeBajaTodos()
        {
            var usuarioId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (usuarioId == null)
            {
                return BadRequest("Usuario no autenticado. No se puede dar de baja el lote sin un usuario válido.");
            }
            await _loteService.DarDeBajaLotesVencidosAsync(usuarioId ?? "sistema");
            return RedirectToAction("Vencidos");
        }

        [HttpPost]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> DarDeBajaUno(int loteId)
        {
            var usuarioId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (usuarioId == null)
            {
                return BadRequest("Usuario no autenticado. No se puede dar de baja el lote sin un usuario válido.");
            }
            await _loteService.DarDeBajaLoteVencidoAsync(loteId, usuarioId ?? "sistema");
            return RedirectToAction("Vencidos");
        }

        public async Task<IActionResult> StockCritico()
        {
            try
            {
                var stock = await _productoService.ObtenerProductosConStockCriticoAsync();
                return View(stock);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener los stocks");
                return View("Error", new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
            }
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
