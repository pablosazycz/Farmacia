using Farmacia.Data;
using Farmacia.Interfaces;
using Farmacia.Models;
using Microsoft.EntityFrameworkCore;

namespace Farmacia.Services
{
    public class ProductoService : IProductoService
    {
        private readonly ApplicationDbContext _context;
        public ProductoService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Producto> CrearProductoAsync(Producto producto)
        {
            producto.FechaAlta = DateTime.Now;
            producto.Activo = true;
            _context.Productos.Add(producto);
            await _context.SaveChangesAsync();
            return producto;
        }

        public async Task<Producto> EditarProductoAsync(Producto producto)
        {
            if (producto.Activo == true)
            {
                producto.FechaBaja = null;
            }
            _context.Productos.Update(producto);
            await _context.SaveChangesAsync();
            return producto;
        }

        public async Task<Producto> EliminarProductoAsync(Producto producto)
        {
            if (producto == null)
            {
                throw new ArgumentNullException(nameof(producto));
            }
            producto.Activo = false;
            producto.FechaBaja = DateTime.Now;
            _context.Productos.Update(producto);
            await _context.SaveChangesAsync();
            return producto;
        }

        public async Task<Producto> ObtenerProductoPorIdAsync(int? id)
        {
            return await _context.Productos
                 .Include(p => p.Droga)
                 .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<List<Producto>> ObtenerProductosActivosAsync()
        {
            return await _context.Productos
                .Where(p => p.Activo)
                .ToListAsync();
        }

        public async Task<List<Producto>> ObtenerProductosAsync()
        {
            return await _context.Productos
                .Include(p => p.Droga)
                .ToListAsync();
        }

        public async Task<List<Producto>> ObtenerProductosPorDrogaAsync(int drogaId)
        {
            return await _context.Productos
              .Where(p => p.DrogaId == drogaId && p.Activo)
              .Include(p => p.Lotes)

              .ToListAsync();
        }

        public async Task<List<Producto>> ObtenerProductosPorNombreAsync(string nombre)
        {
            return await _context.Productos
                .Where(p => p.NombreComercial.Contains(nombre) && p.Activo)
                .Include(p => p.Droga)
                .ToListAsync();
        }

        public async Task<List<Producto>> ObtenerProductosPorDrogaOrdenadosPorVencimientoAsync(int drogaId)
        {
            var productos = await _context.Productos
                  .Include(p => p.Lotes)
                  .Where(p => p.DrogaId == drogaId)
                  .ToListAsync();

            return productos
                 .Where(p => p.Lotes != null && p.Lotes.Any())
                 .OrderBy(p => p.Lotes.Min(l => l.FechaVencimiento))
                 .ToList();
        }

        public async Task<List<Producto>> ObtenerProductosConStockCriticoAsync()
        {
            return await _context.Productos
                .Include(p => p.Lotes)
                .Include(p => p.Droga)
                .Where(p => p.Lotes.Sum(l => l.Cantidad) < p.Droga.StockMinimo && p.Activo)
                .ToListAsync();
        }

        public async Task<List<(Producto producto, int cantidadVendida, int stockActual)>> ObtenerProductosAltaRotacionConStockAsync(DateTime desde, DateTime hasta, int top)
        {
            var ventas = await _context.MovimientosStock
                .Where(m => m.TipoMovimiento == TipoMovimiento.Venta && m.Fecha >= desde && m.Fecha <= hasta)
                .GroupBy(m => m.ProductoId)
                .Select(g => new
                {
                    ProductoId = g.Key,
                    CantidadVendida = g.Sum(m => m.Cantidad)
                })
                .OrderByDescending(x => x.CantidadVendida)
                .Take(top)
                .ToListAsync();

            var productos = await _context.Productos
                .Where(p => ventas.Select(v => v.ProductoId).Contains(p.Id))
                .ToListAsync();

            var hoy = DateTime.Today;
            var lotes = await _context.Lotes
                .Where(l => ventas.Select(v => v.ProductoId).Contains(l.ProductoId) && l.FechaVencimiento >= hoy)
                .ToListAsync();

            var stockPorProducto = lotes
                .GroupBy(l => l.ProductoId)
                .ToDictionary(g => g.Key, g => g.Sum(l => l.Cantidad));

            return ventas
                .Join(productos, v => v.ProductoId, p => p.Id, (v, p) =>
                    (p, v.CantidadVendida, stockPorProducto.ContainsKey(p.Id) ? stockPorProducto[p.Id] : 0))
                .ToList();
        }

        public async Task<List<Producto>> ObtenerProductosParaReposicionAsync()
        {
            return await _context.Productos
                .Include(p => p.Lotes)
                .Include(p => p.Droga)
                .Where(p => p.Lotes.Sum(l => l.Cantidad) < p.Droga.StockMinimo && p.Activo)
                .ToListAsync();
        }

        public async Task<List<Producto>> ObtenerProductosParaPromocionAsync(int diasAVencer , int stockAlto)
        {
            var hoy = DateTime.Today;
            var fechaLimite = hoy.AddDays(diasAVencer);

            return await _context.Productos
                .Include(p => p.Lotes)
                .Where(p => p.Lotes.Any(l =>
                    l.FechaVencimiento >= hoy &&
                    l.FechaVencimiento <= fechaLimite &&
                    l.Cantidad >= stockAlto))
                .ToListAsync();
        
        
        }

        public async Task<List<(Producto producto, int cantidadVendida)>> ObtenerReporteVentasPorProductoAsync(DateTime desde, DateTime hasta)
        {
            var ventas = await _context.MovimientosStock
                .Where(m => m.TipoMovimiento == TipoMovimiento.Venta && m.Fecha >= desde && m.Fecha <= hasta)
                .GroupBy(m => m.ProductoId)
                .Select(g => new
                {
                    ProductoId = g.Key,
                    CantidadVendida = g.Sum(m => m.Cantidad)
                })
                .ToListAsync();

            var productos = await _context.Productos
                .Where(p => ventas.Select(v => v.ProductoId).Contains(p.Id))
                .ToListAsync();

            return ventas
                .Join(productos, v => v.ProductoId, p => p.Id, (v, p) => (p, v.CantidadVendida))
                .ToList();
        }


    }
}
