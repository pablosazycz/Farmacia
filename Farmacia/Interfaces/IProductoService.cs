using Farmacia.Models;
using Microsoft.EntityFrameworkCore;

namespace Farmacia.Interfaces
{
    public interface IProductoService
    {

        Task<Producto> CrearProductoAsync(Producto producto);
        Task<Producto> EditarProductoAsync(Producto producto);
        Task<Producto> EliminarProductoAsync(Producto producto);
        Task<Producto> ObtenerProductoPorIdAsync(int? id);
        Task<List<Producto>> ObtenerProductosAsync();
        Task<List<Producto>> ObtenerProductosPorNombreAsync(string nombre);
        Task<List<Producto>> ObtenerProductosPorDrogaAsync(int drogaId);
        Task<List<Producto>> ObtenerProductosActivosAsync();
        Task<List<Producto>> ObtenerProductosPorDrogaOrdenadosPorVencimientoAsync(int drogaId);
        Task<List<Producto>> ObtenerProductosConStockCriticoAsync();

        Task<List<(Producto producto, int cantidadVendida, int stockActual)>> ObtenerProductosAltaRotacionConStockAsync(DateTime desde, DateTime hasta, int top);

        Task<List<Producto>> ObtenerProductosParaReposicionAsync();
        Task<List<Producto>> ObtenerProductosParaPromocionAsync(int diasAVencer , int stockAlto );
        Task<List<(Producto producto, int cantidadVendida)>> ObtenerReporteVentasPorProductoAsync(DateTime desde, DateTime hasta);


    }
}
