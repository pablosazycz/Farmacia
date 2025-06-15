using Farmacia.Models;

namespace Farmacia.Interfaces
{
    public interface IVentaService
    {
        Task<Venta> CrearVentaAsync(Venta venta, int? promocionId = null);
        Task<Venta> ActualizarVentaAsync(Venta venta);
        Task<bool> EliminarVentaAsync(int id);
        Task<Venta?> ObtenerVentaPorIdAsync(int id);
        Task<List<Venta>> ObtenerVentasAsync();
        Task<List<DetalleVenta>> ObtenerDetallesVentaPorIdAsync(int ventaId);
        Task<List<ClientePromocion>> ObtenerPromocionesAplicadasAsync(int clienteId, int ventaId);
        Task<bool> AplicarPromocionAsync(ClientePromocion clientePromocion);    
    }
}
