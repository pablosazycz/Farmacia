using Farmacia.Models;

namespace Farmacia.Interfaces
{
    public interface IPromocionService
    {
        Task<List<Promocion>> ObtenerPromocionesAsync();
        Task<Promocion?> ObtenerPromocionPorIdAsync(int id);
        Task CrearPromocionAsync(Promocion promocion);
        Task EditarPromocionAsync(Promocion promocion);
        Task EliminarPromocionAsync(int id);
    }
}
