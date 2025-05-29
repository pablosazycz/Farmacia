using Farmacia.Models;

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
    }
}
