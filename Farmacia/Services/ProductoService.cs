using Farmacia.Data;
using Farmacia.Interfaces;
using Farmacia.Models;
using Microsoft.EntityFrameworkCore;

namespace Farmacia.Services
{
    public class ProductoService : IProductoService
    {
        private readonly ApplicationDbContext _context;
        public ProductoService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Producto> CrearProductoAsync(Producto producto)
        {
            producto.FechaAlta = DateTime.Now;
            producto.Activo = true;
            _context.Productos.Add(producto);
            await _context.SaveChangesAsync();
            return producto;
        }

        public async Task<Producto> EditarProductoAsync(Producto producto)
        {
            if (producto.Activo == true)
            {
                producto.FechaBaja = null;
            }
           _context.Productos.Update(producto);
            await _context.SaveChangesAsync();
            return producto;
        }

        public async Task<Producto> EliminarProductoAsync(Producto producto)
        {
            if(producto == null)
            {
                throw new ArgumentNullException(nameof(producto));
            }
            producto.Activo = false;
            producto.FechaBaja = DateTime.Now;
            _context.Productos.Update(producto);
            await _context.SaveChangesAsync();
            return producto;
        }

        public async Task<Producto> ObtenerProductoPorIdAsync(int? id)
        {
            return await _context.Productos
                 .Include(p => p.Droga)
                 .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<List<Producto>> ObtenerProductosActivosAsync()
        {
            return await _context.Productos
                .Where(p => p.Activo)
                .Include(p => p.Droga)
                .ToListAsync();
        }

        public async Task<List<Producto>> ObtenerProductosAsync()
        {
            return await _context.Productos
                .Include(p => p.Droga)
                .ToListAsync();
        }

            public async Task<List<Producto>> ObtenerProductosPorDrogaAsync(int drogaId)
            {
                return await _context.Productos
                  .Where(p => p.DrogaId == drogaId && p.Activo)
              
                  .ToListAsync();
            }

        public async Task<List<Producto>> ObtenerProductosPorNombreAsync(string nombre)
        {
            return await _context.Productos
                .Where(p => p.NombreComercial.Contains(nombre) && p.Activo)
                .Include(p => p.Droga)
                .ToListAsync();
        }
    }
}
