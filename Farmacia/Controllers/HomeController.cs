using System.Diagnostics;
using AspNetCoreGeneratedDocument;
using Farmacia.Interfaces;
using Farmacia.Models;
using Microsoft.AspNetCore.Mvc;

namespace Farmacia.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ILoteService _loteService;

        public HomeController(ILogger<HomeController> logger, ILoteService loteService)
        {
            _logger = logger;
            _loteService = loteService;
        }

        public IActionResult Index()
        {
            return View();
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

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
