using Farmacia.Data;
using Farmacia.Interfaces;
using Farmacia.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

public class VentasController : Controller
{
    private readonly IVentaService _ventaService;
    private readonly UserManager<IdentityUser> _userManager;
    private readonly ApplicationDbContext _context;
    private readonly IProductoService _productoService;
    private readonly IDrogaService _drogaService;
    private readonly IClienteService _clienteService;
    private readonly IPromocionService _promocionService;
    private readonly ITicketService _ticketService;

    public VentasController(IVentaService ventaService, UserManager<IdentityUser> userManager,
        ApplicationDbContext context, IProductoService productoService, IDrogaService drogaService,
        IClienteService clienteService, IPromocionService promocionService, ITicketService ticketService)
    {
        _ventaService = ventaService;
        _userManager = userManager;
        _context = context;
        _productoService = productoService;
        _drogaService = drogaService;
        _clienteService = clienteService;
        _promocionService = promocionService;
        _ticketService = ticketService;
    }

    public async Task<IActionResult> Index()
    {
        var ventas = await _ventaService.ObtenerVentasAsync();
        return View(ventas);
    }

    public async Task<IActionResult> Crear()
    {
        List<Droga> drogas = await _drogaService.ObtenerDrogasActivasAsync();
        ViewBag.Drogas = drogas.ToList();
        List<Producto> producto = await _productoService.ObtenerProductosActivosAsync();
        ViewBag.Productos = producto;
        ViewBag.Clientes = _context.Clientes.ToList();

        var promociones = await _promocionService.ObtenerPromocionesAsync();
        ViewBag.Promociones = promociones.Where(p => p.Activa).ToList();
        return View();
    }
    public async Task<JsonResult> ObtenerSaldoPuntos(int clienteId)
    {
        int saldo = await _clienteService.ObtenerSaldoPuntosAsync(clienteId);
        return Json(new { saldo });
    }

    [HttpPost]
    public async Task<IActionResult> Crear(Venta venta, int? PromocionId)
    {
        venta.Fecha = DateTime.Now;
        venta.UsuarioId = _userManager.GetUserId(User);

        if (venta.DetallesVenta == null || !venta.DetallesVenta.Any())
        {
            ModelState.AddModelError("", "Debe agregar al menos un producto a la venta.");
        }

        if (venta.ClienteId > 0)
        {
            var cliente = await _clienteService.ObtenerClientePorIdAsync(venta.ClienteId);
            ViewBag.ClienteNombreCompleto = cliente != null ? $"{cliente.Nombre} {cliente.Apellido} ({cliente.Dni})" : "";
        }

        if (!ModelState.IsValid)
        {
            List<Producto> producto = await _productoService.ObtenerProductosActivosAsync();
            ViewBag.Productos = producto;
            ViewBag.Clientes = _context.Clientes.ToList();
            var promociones = await _promocionService.ObtenerPromocionesAsync();
            ViewBag.Promociones = promociones.Where(p => p.Activa).ToList();
            return View(venta);
        }

        try
        {
            var ventaGuardada = await _ventaService.CrearVentaAsync(venta, PromocionId);
            return RedirectToAction("VentaConfirmada", new { id = ventaGuardada.Id });
        }
        catch (Exception ex)
        {

            ModelState.AddModelError(string.Empty, ex.Message);

            List<Producto> producto = await _productoService.ObtenerProductosActivosAsync();
            ViewBag.Productos = producto;
            ViewBag.Clientes = _context.Clientes.ToList();
            var promociones = await _promocionService.ObtenerPromocionesAsync();
            ViewBag.Promociones = promociones.Where(p => p.Activa).ToList();

            if (venta.ClienteId > 0)
            {
                var cliente = await _clienteService.ObtenerClientePorIdAsync(venta.ClienteId);
                ViewBag.ClienteNombreCompleto = cliente != null ? $"{cliente.Nombre} {cliente.Apellido} ({cliente.Dni})" : "";
            }

            return View(venta);
        }
    }

    public async Task<IActionResult> Details(int id)
    {
        var venta = await _ventaService.ObtenerVentaPorIdAsync(id);
        if (venta == null) return NotFound();

        // Cargar detalles con producto incluido
        venta.DetallesVenta = await _ventaService.ObtenerDetallesVentaPorIdAsync(id);

        var promociones = await _ventaService.ObtenerPromocionesAplicadasAsync(venta.ClienteId, venta.Id);
        ViewBag.PromocionAplicada = promociones.FirstOrDefault();

        return View(venta);
    }

    public async Task<IActionResult> Ticket(int id)
    {
        var venta = await _ventaService.ObtenerVentaPorIdAsync(id);
        if (venta == null) return NotFound();

        // Asegúrate de que los detalles incluyan el producto
        venta.DetallesVenta = await _ventaService.ObtenerDetallesVentaPorIdAsync(id);

        var cliente = await _clienteService.ObtenerClientePorIdAsync(venta.ClienteId);

        var pdf = _ticketService.GenerarTicketVentaPdf(venta, cliente);
        return File(pdf, "application/pdf");
    }

    public async Task<IActionResult> Edit(int id)
    {
        var venta = await _ventaService.ObtenerVentaPorIdAsync(id);
        if (venta == null) return NotFound();

        // Cargar datos auxiliares si es necesario (clientes, productos, etc.)
        ViewBag.Clientes = _context.Clientes.ToList();
        ViewBag.Productos = await _productoService.ObtenerProductosActivosAsync();

        return View(venta);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Venta venta)
    {
        if (id != venta.Id) return BadRequest();

        if (!ModelState.IsValid)
        {
            ViewBag.Clientes = _context.Clientes.ToList();
            ViewBag.Productos = await _productoService.ObtenerProductosActivosAsync();
            return View(venta);
        }

        await _ventaService.ActualizarVentaAsync(venta);
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int id)
    {
        var venta = await _ventaService.ObtenerVentaPorIdAsync(id);
        if (venta == null) return NotFound();
        return View(venta);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        await _ventaService.EliminarVentaAsync(id);
        return RedirectToAction(nameof(Index));
    }

    public IActionResult VentaConfirmada(int id)
    {
        ViewBag.VentaId = id;
        return View();
    }
}


