using Farmacia.Models;

namespace Farmacia.Interfaces
{
    public interface ILoteService
    {
        Task<Lote> CrearLoteAsync(Lote lote);
        Task<Lote> AgregarNuevoLoteAsync(int productoId, int cantidad, string codigoLote, DateTime fechaVencimiento);
        Task DescontarStockDesdeLotesAsync(int productoId, int cantidad);
        Task<Lote> EditarLoteAsync(Lote lote);
        Task<bool> EliminarLoteAsync(int loteId);
        Task<Lote> ObtenerLotePorIdAsync(int id);
        Task<List<Lote>> ObtenerLotesPorProductoIdAsync(int productoId);
        Task<List<Lote>> ObtenerLotesPorVencimientoAsync(DateTime hastaFecha);
        Task<List<Lote>> ObtenerTodosLosLotesAsync();
    }
}
