using Farmacia.Data;
using Farmacia.Models;
using Farmacia.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Farmacia.Services
{

    public class VentaService : IVentaService
    {
        private readonly ApplicationDbContext _context;

        public VentaService(ApplicationDbContext context)
        {
            _context = context;
        }


        public Task<Venta> ActualizarVentaAsync(Venta venta)
        {
            throw new NotImplementedException();
        }

        public Task<bool> AplicarPromocionAsync(ClientePromocion clientePromocion)
        {
            throw new NotImplementedException();
        }

        public Task<Venta> CrearVentaAsync(Venta venta)
        {
            throw new NotImplementedException();
        }

        public Task<bool> EliminarVentaAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<List<DetalleVenta>> ObtenerDetallesVentaPorIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<List<ClientePromocion>> ObtenerPromocionesAplicadasAsync(int clienteId, int ventaId)
        {
            throw new NotImplementedException();
        }

        public Task<Venta> ObtenerVentaPorIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<List<Venta>> ObtenerVentasAsync()
        {
            throw new NotImplementedException();
        }
    }
}
