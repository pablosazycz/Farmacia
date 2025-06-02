using Farmacia.Data;
using Farmacia.Interfaces;
using Farmacia.Models;
using Microsoft.EntityFrameworkCore;

namespace Farmacia.Services
{
    public class LoteService : ILoteService
    {
        private readonly ApplicationDbContext _context;

        public LoteService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Lote> CrearLoteAsync(Lote lote)
        {
            _context.Lotes.Add(lote);
            await _context.SaveChangesAsync();
            return lote;
        }

        public async Task<Lote> EditarLoteAsync(Lote lote)
        {
            _context.Lotes.Update(lote);
            await _context.SaveChangesAsync();
            return lote;

        }

        public async Task<bool> EliminarLoteAsync(int loteId)
        {
            Lote lote = await _context.Lotes.FindAsync(loteId);
            if (lote == null)
            {
                return false;
            }

            _context.Lotes.Remove(lote);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<Lote> ObtenerLotePorIdAsync(int id)
        {
            return await _context.Lotes.Include(l => l.Producto).FirstOrDefaultAsync(l => l.Id == id);
        }

        public async Task<List<Lote>> ObtenerLotesPorProductoIdAsync(int productoId)
        {
            return await _context.Lotes
                .Where(l => l.ProductoId == productoId)
                .ToListAsync();
        }

        public async Task<List<Lote>> ObtenerLotesPorVencimientoAsync(DateTime hastaFecha)
        {
            return await _context.Lotes
                   .Where(l => l.FechaVencimiento <= hastaFecha)
                   .ToListAsync();
        }

        public async Task<List<Lote>> ObtenerTodosLosLotesAsync()
        {
            return await _context.Lotes.ToListAsync();
        }

        public async Task<Lote> AgregarNuevoLoteAsync(int productoId, int cantidad, string codigoLote, DateTime fechaVencimiento)
        {
            Lote nuevoLote = new Lote
            {
                ProductoId = productoId,
                Cantidad = cantidad,
                CodigoLote = codigoLote,
                FechaVencimiento = fechaVencimiento
            };

            _context.Lotes.Add(nuevoLote);
            await _context.SaveChangesAsync();
            return nuevoLote;
        }

        public async Task DescontarStockDesdeLotesAsync(int productoId, int cantidad)
        {
            List<Lote> lotes = await _context.Lotes
                .Where(l => l.ProductoId == productoId && l.Cantidad > 0 && l.FechaVencimiento >= DateTime.Today)
                .OrderBy(l => l.FechaVencimiento)
                .ToListAsync();

            int restante = cantidad;

            foreach (Lote lote in lotes)
            {
                if (restante <= 0)
                    break;

                int aDescontar = Math.Min(lote.Cantidad, restante);
                lote.Cantidad -= aDescontar;
                restante -= aDescontar;
            }

            if (restante > 0)
            {
                throw new InvalidOperationException("Stock insuficiente en los lotes del producto.");
            }

            await _context.SaveChangesAsync();
        }

        public async Task<List<Lote>> ObtenerLotesProximosAVencerAsync(int dias)
        {
            var hoy = DateTime.Today;
            var fechaLimite = hoy.AddDays(dias);
            return await _context.Lotes
                .Where(l => l.FechaVencimiento > hoy && l.FechaVencimiento <= fechaLimite && l.Cantidad > 0)
                .Include(l => l.Producto)
                .ToListAsync();
        }

        public async Task<List<Lote>> ObtenerLotesVencidosAsync()
        {
            var hoy = DateTime.Today;

            return await _context.Lotes
                .Where(l => l.FechaVencimiento < hoy && l.Cantidad > 0)
                .Include(l => l.Producto)
                .ToListAsync();
        }
    }
}
