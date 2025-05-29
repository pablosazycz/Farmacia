using Farmacia.Data;
using Farmacia.Models;
using Farmacia.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Farmacia.Services
{

    public class VentaService : IVentaService
    {

        private readonly ApplicationDbContext _context;
        private readonly IMovimientoStockService _movimientoService;
        public VentaService(ApplicationDbContext context, IMovimientoStockService movimientoService)
        {
            _context = context;
            _movimientoService = movimientoService;
        }


        public Task<Venta> ActualizarVentaAsync(Venta venta)
        {
            throw new NotImplementedException();
        }

        public Task<bool> AplicarPromocionAsync(ClientePromocion clientePromocion)
        {
            throw new NotImplementedException();
        }


        public async Task<Venta> CrearVentaAsync(Venta venta)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                venta.Fecha = DateTime.Now;
                venta.Total = venta.DetallesVenta.Sum(d => d.Subtotal);

                _context.Ventas.Add(venta);
                await _context.SaveChangesAsync();

                foreach (var detalle in venta.DetallesVenta)
                {

                    var movimiento = new MovimientoStock
                    {
                        ProductoId = detalle.ProductoId,
                        DrogaId = await ObtenerDrogaIdPorProducto(detalle.ProductoId),
                        Cantidad = detalle.Cantidad,
                        CodigoLote = await ObtenerLoteDisponible(detalle.ProductoId, detalle.Cantidad),
                        TipoMovimiento = TipoMovimiento.Venta,
                        UsuarioId = venta.UsuarioId
                    };

                    await _movimientoService.CrearMovimientoAsync(movimiento);
                }

                await _context.SaveChangesAsync();
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

        public Task<bool> EliminarVentaAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<List<DetalleVenta>> ObtenerDetallesVentaPorIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<List<ClientePromocion>> ObtenerPromocionesAplicadasAsync(int clienteId, int ventaId)
        {
            throw new NotImplementedException();
        }

        public Task<Venta> ObtenerVentaPorIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<List<Venta>> ObtenerVentasAsync()
        {
            throw new NotImplementedException();
        }
    }
}
