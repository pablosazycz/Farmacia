using Farmacia.Data;
using Farmacia.Interfaces;
using Farmacia.Models;
using Microsoft.EntityFrameworkCore;

namespace Farmacia.Services
{

    public class VentaService : IVentaService
    {

        private readonly ApplicationDbContext _context;
        private readonly IMovimientoStockService _movimientoService;
        private readonly IClienteService _clienteService;
        public readonly ILoteService _loteService;

        public VentaService(ApplicationDbContext context, IMovimientoStockService movimientoService, IClienteService clienteService, ILoteService loteService)
        {
            _context = context;
            _movimientoService = movimientoService;
            _clienteService = clienteService;
            _loteService = loteService;
        }


        public async Task<Venta> ActualizarVentaAsync(Venta venta)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            var ventaExistente = await _context.Ventas
                .Include(v => v.DetallesVenta)
                .Include(v => v.Cliente)
                .FirstOrDefaultAsync(v => v.Id == venta.Id);

            if (ventaExistente == null)
                throw new Exception("Venta no encontrada");

            // 1. Revertir movimientos de stock anteriores (devolver stock)
            var movimientosAnteriores = await _context.MovimientosStock
                .Where(m => m.TipoMovimiento == TipoMovimiento.Venta && m.VentaId == ventaExistente.Id)
                .ToListAsync();

            foreach (var mov in movimientosAnteriores)
            {
                // Devuelve stock al lote y a la droga
                var lote = await _context.Lotes.FirstOrDefaultAsync(l => l.CodigoLote == mov.CodigoLote && l.ProductoId == mov.ProductoId);
                if (lote != null)
                    lote.Cantidad += mov.Cantidad;

                var producto = await _context.Productos.FindAsync(mov.ProductoId);
                if (producto != null)
                {
                    var droga = await _context.Drogas.FindAsync(producto.DrogaId);
                    if (droga != null)
                        droga.Stock += mov.Cantidad * producto.CantidadPresentacion;
                }
            }
            _context.MovimientosStock.RemoveRange(movimientosAnteriores);

            // 2. Actualizar cliente
            ventaExistente.ClienteId = venta.ClienteId;

            // 3. Actualizar detalles
            // Eliminar detalles que ya no están
            var detallesAEliminar = ventaExistente.DetallesVenta
                .Where(d => !venta.DetallesVenta.Any(nd => nd.Id == d.Id))
                .ToList();
            _context.DetallesVenta.RemoveRange(detallesAEliminar);

            // Actualizar o agregar detalles
            foreach (var detalleNuevo in venta.DetallesVenta)
            {
                var detalleExistente = ventaExistente.DetallesVenta.FirstOrDefault(d => d.Id == detalleNuevo.Id);
                if (detalleExistente != null)
                {
                    detalleExistente.ProductoId = detalleNuevo.ProductoId;
                    detalleExistente.Cantidad = detalleNuevo.Cantidad;
                    detalleExistente.PrecioUnitario = detalleNuevo.PrecioUnitario;
                    detalleExistente.Subtotal = detalleNuevo.Subtotal;
                }
                else
                {
                    detalleNuevo.VentaId = ventaExistente.Id;
                    _context.DetallesVenta.Add(detalleNuevo);
                }
            }

            await _context.SaveChangesAsync();

            // 4. Validar stock y crear nuevos movimientos de stock
            var detallesActualizados = await _context.DetallesVenta
                .Where(d => d.VentaId == ventaExistente.Id)
                .ToListAsync();

            foreach (var detalle in detallesActualizados)
            {
                // Validar stock disponible
                var loteCodigo = await ObtenerLoteDisponible(detalle.ProductoId, detalle.Cantidad);
                var producto = await _context.Productos.FindAsync(detalle.ProductoId);
                if (producto == null)
                    throw new Exception("Producto no encontrado");

                var droga = await _context.Drogas.FindAsync(producto.DrogaId);
                if (droga == null)
                    throw new Exception("Droga no encontrada");

                var lote = await _context.Lotes.FirstOrDefaultAsync(l => l.CodigoLote == loteCodigo && l.ProductoId == detalle.ProductoId);
                if (lote == null || lote.Cantidad < detalle.Cantidad)
                    throw new Exception("No hay stock suficiente para el producto.");

                // Descontar stock
                lote.Cantidad -= detalle.Cantidad;
                droga.Stock -= detalle.Cantidad * producto.CantidadPresentacion;



                // Registrar movimiento de stock
                var movimiento = new MovimientoStock
                {
                    ProductoId = detalle.ProductoId,
                    DrogaId = producto.DrogaId,
                    Cantidad = detalle.Cantidad,
                    CodigoLote = loteCodigo,
                    TipoMovimiento = TipoMovimiento.Venta,
                    UsuarioId = ventaExistente.UsuarioId,
                    Fecha = DateTime.Now,
                    VentaId = ventaExistente.Id
                };
                _context.MovimientosStock.Add(movimiento);
            }

            // 5. Recalcular total y descuento
            ventaExistente.Total = detallesActualizados.Sum(d => d.Subtotal);

            // Recalcular descuento si hay promoción aplicada
            var promoAplicada = await _context.ClientePromociones
                .Include(cp => cp.Promocion)
                .FirstOrDefaultAsync(cp => cp.VentaId == ventaExistente.Id);

            if (promoAplicada != null && promoAplicada.Promocion != null && promoAplicada.Promocion.Descuento > 0)
            {
                var descuento = ventaExistente.Total * promoAplicada.Promocion.Descuento;
                ventaExistente.DescuentoAplicado = descuento;
                ventaExistente.Total -= descuento;
            }
            else
            {
                ventaExistente.DescuentoAplicado = 0;
            }

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            return ventaExistente;
        }

        public Task<bool> AplicarPromocionAsync(ClientePromocion clientePromocion)
        {
            throw new NotImplementedException();
        }

        public async Task<Venta> CrearVentaAsync(Venta venta, int? promocionId = null)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                venta.Fecha = DateTime.Now;

                // Validación de stock y precios
                foreach (var detalle in venta.DetallesVenta)
                {
                    var producto = await _context.Productos.FindAsync(detalle.ProductoId);
                    if (producto == null)
                        throw new Exception($"Producto con ID {detalle.ProductoId} no encontrado.");

                    // Validar stock suficiente SOLO en el lote seleccionado
                    var lote = await _context.Lotes
                        .FirstOrDefaultAsync(l => l.ProductoId == detalle.ProductoId && l.CodigoLote == detalle.CodigoLote);

                    if (lote == null)
                        throw new Exception($"No se encontró el lote '{detalle.CodigoLote}' para el producto '{producto.NombreComercial}'.");

                    if (lote.Cantidad < detalle.Cantidad)
                        throw new Exception($"No hay stock suficiente en el lote '{detalle.CodigoLote}' para el producto '{producto.NombreComercial}'. Stock disponible: {lote.Cantidad}.");
                }

                // Recalcula los subtotales y el total en el backend
                foreach (var detalle in venta.DetallesVenta)
                {
                    var producto = await _context.Productos.FindAsync(detalle.ProductoId);
                    if (producto == null)
                        throw new Exception("Producto no encontrado");

                    detalle.PrecioUnitario = producto.PrecioUnitario;
                    detalle.Subtotal = detalle.Cantidad * detalle.PrecioUnitario;
                }

                // 1. Consultar puntos del cliente
                int puntosCliente = await _clienteService.ObtenerSaldoPuntosAsync(venta.ClienteId);

                // 2. Buscar la mejor promoción disponible según puntos
                Promocion? promo = null;
                if (promocionId.HasValue)
                {
                    promo = await _context.Promociones
                        .FirstOrDefaultAsync(p => p.Id == promocionId.Value && p.Activa && p.CantidadMinima <= puntosCliente);
                }

                venta.Total = venta.DetallesVenta.Sum(d => d.Subtotal);

                if (promo != null)
                {
                    // Aplica la promoción seleccionada
                    var descuento = venta.Total * promo.Descuento;
                    venta.DescuentoAplicado = descuento;
                    venta.Total -= descuento;
                }
                else
                {
                    venta.DescuentoAplicado = 0;
                }

                _context.Ventas.Add(venta);
                await _context.SaveChangesAsync();

                // Registrar la relación con la promoción si corresponde
                if (promo != null)
                {
                    var clientePromocion = new ClientePromocion
                    {
                        ClienteId = venta.ClienteId,
                        PromocionId = promo.Id,
                        FechaAplicacion = DateTime.Now,
                        VentaId = venta.Id
                    };

                    // Descontar puntos por la promoción (registra movimiento negativo en ClientePuntos)
                    if (promo.CantidadMinima > 0)
                    {
                        await _clienteService.CanjearPuntosAsync(
                        venta.ClienteId,
                        promo.CantidadMinima.Value,
                        $"Canje por promoción: {promo.Nombre}",
                        promo.Id,
                        venta.Id
                         );
                    }


                    _context.ClientePromociones.Add(clientePromocion);
                    await _context.SaveChangesAsync();
                }

                // Descontar stock y registrar movimiento de stock por el lote seleccionado
                foreach (var detalle in venta.DetallesVenta)
                {
                    var lote = await _context.Lotes
                        .FirstOrDefaultAsync(l => l.ProductoId == detalle.ProductoId && l.CodigoLote == detalle.CodigoLote);

                    if (lote == null || lote.Cantidad < detalle.Cantidad)
                        throw new Exception($"No hay stock suficiente en el lote '{detalle.CodigoLote}' para el producto.");


                    var movimiento = new MovimientoStock
                    {
                        ProductoId = detalle.ProductoId,
                        DrogaId = await ObtenerDrogaIdPorProducto(detalle.ProductoId),
                        Cantidad = detalle.Cantidad,
                        CodigoLote = detalle.CodigoLote,
                        TipoMovimiento = TipoMovimiento.Venta,
                        UsuarioId = venta.UsuarioId,
                        Fecha = DateTime.Now,
                        VentaId = venta.Id
                    };

                    await _movimientoService.CrearMovimientoAsync(movimiento);
                }

                // Sumar puntos por la compra
                int puntos = (int)(venta.Total / 100);
                if (puntos > 0)
                {
                    await _clienteService.SumarPuntosAsync(venta.ClienteId, puntos, "Compra", promo?.Id, venta.Id);
                }

                await transaction.CommitAsync();

                return venta;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
        private async Task<int> ObtenerDrogaIdPorProducto(int productoId)
        {
            var producto = await _context.Productos.FindAsync(productoId);
            return producto?.DrogaId ?? throw new Exception("Producto no encontrado");
        }

        private async Task<string> ObtenerLoteDisponible(int productoId, int cantidad)
        {
            var lote = await _context.Lotes
                .Where(l => l.ProductoId == productoId && l.Cantidad >= cantidad)
                .OrderBy(l => l.FechaVencimiento)
                .FirstOrDefaultAsync();

            return lote?.CodigoLote ?? throw new Exception("No hay stock suficiente para el producto.");
        }

        public async Task<bool> EliminarVentaAsync(int id)
        {
            var venta = await _context.Ventas
                .Include(v => v.DetallesVenta)
                .FirstOrDefaultAsync(v => v.Id == id);

            if (venta == null)
                return false;

            // Elimina ClientePromociones relacionadas
            var clientePromos = await _context.ClientePromociones
                .Where(cp => cp.VentaId == id)
                .ToListAsync();
            _context.ClientePromociones.RemoveRange(clientePromos);

            // Elimina ClientePuntos relacionados
            var clientePuntos = await _context.ClientePuntos
                .Where(cp => cp.VentaId == id)
                .ToListAsync();
            _context.ClientePuntos.RemoveRange(clientePuntos);

            // Elimina detalles primero
            _context.DetallesVenta.RemoveRange(venta.DetallesVenta);
            _context.Ventas.Remove(venta);

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<DetalleVenta>> ObtenerDetallesVentaPorIdAsync(int ventaId)
        {
            return await _context.DetallesVenta
                .Where(d => d.VentaId == ventaId)
                .Include(d => d.Producto)
                .ToListAsync();
        }

        public async Task<List<ClientePromocion>> ObtenerPromocionesAplicadasAsync(int clienteId, int ventaId)
        {
            return await _context.ClientePromociones
                .Where(cp => cp.ClienteId == clienteId && cp.VentaId == ventaId)
                .Include(cp => cp.Promocion)
                .ToListAsync();
        }

        public async Task<Venta> ObtenerVentaPorIdAsync(int id)
        {
            return await _context.Ventas
                .Include(v => v.Cliente)
                .Include(v => v.DetallesVenta)
                .FirstOrDefaultAsync(v => v.Id == id);
        }

        public async Task<List<Venta>> ObtenerVentasAsync()
        {
            return await _context.Ventas
                .Include(v => v.Cliente)
                .Include(v => v.DetallesVenta)
                .OrderByDescending(v => v.Fecha)
                .ToListAsync();
        }
    }
}
