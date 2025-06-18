using Farmacia.Data;
using Farmacia.Interfaces;
using Farmacia.Models;
using Microsoft.EntityFrameworkCore;

namespace Farmacia.Services
{
    public class PromocionService : IPromocionService
    {
        private readonly ApplicationDbContext _context;

        public PromocionService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Promocion>> ObtenerPromocionesAsync()
        {
            return await _context.Promociones.ToListAsync();
        }

        public async Task<Promocion?> ObtenerPromocionPorIdAsync(int id)
        {
            return await _context.Promociones.FindAsync(id);
        }

        public async Task CrearPromocionAsync(Promocion promocion)
        {
            if (promocion.Descuento > 1)
                promocion.Descuento = promocion.Descuento / 100m;

            _context.Promociones.Add(promocion);
            await _context.SaveChangesAsync();
        }

        public async Task EditarPromocionAsync(Promocion promocion)
        {
            if (promocion.Descuento > 1)
                promocion.Descuento = promocion.Descuento / 100m;

            _context.Promociones.Update(promocion);
            await _context.SaveChangesAsync();
        }

        public async Task EliminarPromocionAsync(int id)
        {
            var promocion = await _context.Promociones.FindAsync(id);
            if (promocion != null)
            {
                _context.Promociones.Remove(promocion);
                await _context.SaveChangesAsync();
            }
        }
    }
}
