using Farmacia.Data;
using Farmacia.Interfaces;
using Farmacia.Models;
using Farmacia.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

public class VentasController : Controller
{
    private readonly IVentaService _ventaService;
    private readonly UserManager<IdentityUser> _userManager;
    private readonly ApplicationDbContext _context;
    private readonly IProductoService _productoService;
    private readonly IDrogaService _drogaService;

    public VentasController(IVentaService ventaService, UserManager<IdentityUser> userManager,
        ApplicationDbContext context, IProductoService productoService, IDrogaService drogaService)
    {
        _ventaService = ventaService;
        _userManager = userManager;
        _context = context;
        _productoService = productoService;
        _drogaService = drogaService;
    }

    public async Task<IActionResult> Crear()
    {
        List<Droga> drogas = await _drogaService.ObtenerDrogasActivasAsync();
        ViewBag.Drogas = drogas.ToList();
        List<Producto> producto = await _productoService.ObtenerProductosActivosAsync();
        ViewBag.Productos = producto;
        ViewBag.Clientes = _context.Clientes.ToList();

        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Crear(Venta venta)
    {
        venta.Fecha = DateTime.Now;

        if (!ModelState.IsValid)
        {
            List<Producto> producto = await _productoService.ObtenerProductosActivosAsync();
            ViewBag.Productos = producto;
            ViewBag.Clientes = _context.Clientes.ToList();
            return View(venta);
        }

        venta.UsuarioId = _userManager.GetUserId(User);

        await _ventaService.CrearVentaAsync(venta);
        return RedirectToAction("Index");
    }


}
