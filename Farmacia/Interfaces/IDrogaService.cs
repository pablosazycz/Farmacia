using Farmacia.Models;

namespace Farmacia.Interfaces
{
    public interface IDrogaService
    {
        Task<Droga> CrearDrogaAsync(Droga droga);
        Task<Droga> EditarDrogaAsync(Droga droga);
        Task<Droga> EliminarDrogaAsync(Droga droga);
        Task<Droga> ObtenerDrogaPorIdAsync(int id);
        Task<List<Droga>> ObtenerDrogasAsync();
        Task<List<Droga>> ObtenerDrogasPorNombreAsync(string nombre);
        Task<List<Droga>> ObtenerDrogasPorRequiereRecetaAsync(bool requiereReceta);
        Task<List<Droga>> ObtenerDrogasActivasAsync();
        Task<List<Droga>> BuscarDrogasAsync(string term);

    }
}
