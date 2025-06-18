using Farmacia.Models;

namespace Farmacia.Interfaces
{
    public interface IClienteService
    {
        Task<List<Cliente>> ObtenerClientesAsync();
        Task<Cliente?> ObtenerClientePorIdAsync(int id);
        Task CrearClienteAsync(Cliente cliente);
        Task EditarClienteAsync(Cliente cliente);
        Task EliminarClienteAsync(int id);

        Task SumarPuntosAsync(int clienteId, int puntos, string motivo, int? promocionId = null, int? ventaId = null);
        Task CanjearPuntosAsync(int clienteId, int puntos, string motivo, int? promocionId = null, int? ventaId = null);
        Task<int> ObtenerSaldoPuntosAsync(int clienteId);
        Task<List<ClientePunto>> ObtenerHistorialPuntosAsync(int clienteId);

        Task<List<Cliente>> BuscarClientesAsync(string term);

    }
}
