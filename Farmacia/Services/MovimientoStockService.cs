using Farmacia.Data;
using Farmacia.Interfaces;
using Farmacia.Models;
using Microsoft.EntityFrameworkCore;

namespace Farmacia.Services
{
    public class MovimientoStockService : IMovimientoStockService
    {
        private readonly IDrogaService _drogaService;
        private readonly IProductoService _productoService;
        private readonly ILoteService _loteService;
        private readonly ApplicationDbContext _context;

        public MovimientoStockService(
            IDrogaService drogaService,
            IProductoService productoService,
            ILoteService loteService,
            ApplicationDbContext context)
        {
            _drogaService = drogaService;
            _productoService = productoService;
            _loteService = loteService;
            _context = context;
        }

        public async Task<MovimientoStock> CrearMovimientoAsync(MovimientoStock movimiento)
        {


            Droga droga = await _drogaService.ObtenerDrogaPorIdAsync(movimiento.DrogaId);
            if (droga == null)
                throw new ArgumentException("La droga especificada no existe.");
            Producto producto = await _productoService.ObtenerProductoPorIdAsync(movimiento.ProductoId);
            if (producto == null)
                throw new ArgumentException("La droga o el producto no existen.");

            if (movimiento.Fecha == default)
                movimiento.Fecha = DateTime.Now;

            if (movimiento.Cantidad < 0)
                throw new ArgumentException("La cantidad no puede ser negativa.");

            if (string.IsNullOrWhiteSpace(movimiento.Lote.CodigoLote))
            {
                throw new ArgumentException("El código de lote es obligatorio.");
            }

            Lote loteExistente = await _context.Lotes
                .FirstOrDefaultAsync(l => l.CodigoLote == movimiento.CodigoLote && l.ProductoId == movimiento.ProductoId);

            switch (movimiento.TipoMovimiento)
            {
                case TipoMovimiento.Compra:
                    // Si el lote no existe, lo creamos
                    if (loteExistente == null)
                    {
                        Lote nuevoLote = new Lote
                        {
                            CodigoLote = movimiento.CodigoLote,
                            FechaVencimiento = movimiento.Fecha,
                            Cantidad = movimiento.Cantidad,
                            ProductoId = (int)movimiento.ProductoId
                        };

                        _context.Lotes.Add(nuevoLote);
                        await _context.SaveChangesAsync();
                        movimiento.Lote.Id = nuevoLote.Id;
                        movimiento.Lote = nuevoLote;

                        droga.Stock += movimiento.Cantidad;
                        _context.Drogas.Update(droga);
                        await _context.SaveChangesAsync();
                    }
                    else
                    {
                        // Si el lote ya existe, actualizamos la cantidad
                        loteExistente.Cantidad += movimiento.Cantidad;
                        _context.Lotes.Update(loteExistente);
                        await _context.SaveChangesAsync();
                        movimiento.Lote.Id = loteExistente.Id;
                        movimiento.Lote = loteExistente;

                        droga.Stock += movimiento.Cantidad;
                        _context.Drogas.Update(droga);
                        await _context.SaveChangesAsync();
                    }

                    break;
                case TipoMovimiento.Venta:
                    if (loteExistente != null)
                    {
                        if (loteExistente.Cantidad >= movimiento.Cantidad)
                        {
                            loteExistente.Cantidad -= movimiento.Cantidad;
                            _context.Lotes.Update(loteExistente);
                            await _context.SaveChangesAsync();
                            movimiento.Lote.Id = loteExistente.Id;
                            movimiento.Lote = loteExistente;

                            droga.Stock -= movimiento.Cantidad;
                            _context.Drogas.Update(droga);
                            await _context.SaveChangesAsync();

                        }
                        else
                        {
                            throw new ArgumentException("No hay suficiente cantidad en el lote para realizar la venta.");
                        }

                    }

                    else
                    {
                        throw new ArgumentException("El lote especificado no existe para la venta.");
                    }
                    break;
            }

            _context.MovimientosStock.Add(movimiento);
            await _context.SaveChangesAsync();

            return movimiento;

        }

        public async Task<List<MovimientoStock>> ObtenerMovimientosPorDrogaIdAsync(int drogaId)
        {
            return await _context.MovimientosStock
                        .Where(m => m.DrogaId == drogaId)
                        .Include(m => m.Droga)  
                        .Include(m => m.Lote)
                        .ToListAsync();
        }

        public async Task<List<MovimientoStock>> ObtenerMovimientosPorFechaAsync(DateTime fecha)
        {
            return await _context.MovimientosStock
                        .Where(m => m.Fecha.Date == fecha.Date)  
                        .Include(m => m.Droga)  
                        .Include(m => m.Lote)   
                        .ToListAsync();
        }

        public async Task<List<MovimientoStock>> ObtenerMovimientosPorTipoAsync(TipoMovimiento tipoMovimiento)
        {
            return await _context.MovimientosStock
                          .Where(m => m.TipoMovimiento == tipoMovimiento)
                          .Include(m => m.Droga) 
                          .Include(m => m.Lote)
                          .ToListAsync();
        }
    }
}
