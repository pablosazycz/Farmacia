using Farmacia.Data;
using Farmacia.Interfaces;
using Farmacia.Models;
using Microsoft.EntityFrameworkCore;

namespace Farmacia.Services
{
    public class ClienteService : IClienteService
    {
        private readonly ApplicationDbContext _context;

        public ClienteService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Cliente>> BuscarClientesAsync(string term)
        {
            if (string.IsNullOrWhiteSpace(term))
                return new List<Cliente>();

            return await _context.Clientes
                .Where(c =>
                    c.Nombre.Contains(term) ||
                    c.Apellido.Contains(term) ||
                    c.Dni.Contains(term)
                )
                .OrderBy(c => c.Nombre)
                .Take(20)
                .ToListAsync();
        }

        public async Task<List<Cliente>> ObtenerClientesAsync()
        {
            return await _context.Clientes.ToListAsync();
        }

        public async Task<Cliente?> ObtenerClientePorIdAsync(int id)
        {
            return await _context.Clientes.FindAsync(id);
        }

        public async Task CrearClienteAsync(Cliente cliente)
        {
            _context.Clientes.Add(cliente);
            await _context.SaveChangesAsync();
        }

        public async Task EditarClienteAsync(Cliente cliente)
        {
            _context.Clientes.Update(cliente);
            await _context.SaveChangesAsync();
        }

        public async Task EliminarClienteAsync(int id)
        {
            var cliente = await _context.Clientes.FindAsync(id);
            if (cliente != null)
            {
                _context.Clientes.Remove(cliente);
                await _context.SaveChangesAsync();
            }
        }

        public async Task SumarPuntosAsync(int clienteId, int puntos, string motivo, int? promocionId = null, int? ventaId = null)
        {
            var movimiento = new ClientePunto
            {
                ClienteId = clienteId,
                Puntos = puntos,
                Motivo = motivo,
                PromocionId = promocionId,
                VentaId = ventaId,
                Fecha = DateTime.Now
            };
            _context.ClientePuntos.Add(movimiento);
            await _context.SaveChangesAsync();
        }

        public async Task CanjearPuntosAsync(int clienteId, int puntos, string motivo, int? promocionId = null, int? ventaId = null)
        {
            var movimiento = new ClientePunto
            {
                ClienteId = clienteId,
                Puntos = -Math.Abs(puntos),
                Motivo = motivo,
                PromocionId = promocionId,
                VentaId = ventaId,
                Fecha = DateTime.Now
            };
            _context.ClientePuntos.Add(movimiento);
            await _context.SaveChangesAsync();
        }

        public async Task<int> ObtenerSaldoPuntosAsync(int clienteId)
        {
            return await _context.ClientePuntos
                .Where(cp => cp.ClienteId == clienteId)
                .SumAsync(cp => cp.Puntos);
        }

        public async Task<List<ClientePunto>> ObtenerHistorialPuntosAsync(int clienteId)
        {
            return await _context.ClientePuntos
                .Where(cp => cp.ClienteId == clienteId)
                .OrderByDescending(cp => cp.Fecha)
                .ToListAsync();
        }



    }
}
