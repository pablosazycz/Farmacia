using Farmacia.Models;

namespace Farmacia.Interfaces
{
    public interface IMovimientoStockService
    {
        Task<MovimientoStock> CrearMovimientoAsync(MovimientoStock movimiento);
        Task<List<MovimientoStock>> ObtenerMovimientosPorDrogaIdAsync(int drogaId);
        Task<List<MovimientoStock>> ObtenerMovimientosPorTipoAsync(TipoMovimiento tipoMovimiento);
        Task<List<MovimientoStock>> ObtenerMovimientosPorFechaAsync(DateTime fecha);
    }
}
