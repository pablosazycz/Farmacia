using Farmacia.Data;
using Farmacia.Interfaces;
using Farmacia.Models;
using Microsoft.EntityFrameworkCore;

namespace Farmacia.Services
{
    public class DrogaService : IDrogaService
    {
        private readonly ApplicationDbContext _context;

        public DrogaService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Droga> CrearDrogaAsync(Droga droga)
        {
            try
            {   droga.FechaAlta = DateTime.Now;
                droga.Activo = true;
                _context.Drogas.Add(droga);
                await _context.SaveChangesAsync();
                return droga;
            }
            catch (DbUpdateException ex)
            {
                // Manejo de excepciones
                throw new Exception("Error al crear la droga", ex);
            }
        }

        public async Task<Droga> EditarDrogaAsync(Droga droga)
        {
            try
            {
                if (droga.Activo == true)
                {
                    droga.FechaBaja = null;
                }

                _context.Drogas.Update(droga);
                await _context.SaveChangesAsync();
                return droga;
            }
            catch (DbUpdateException ex)
            {
                // Manejo de excepciones
                throw new Exception("Error al editar la droga", ex);
            }
        }


        public async Task<Droga> EliminarDrogaAsync(Droga droga)
        {
            try
            {
                if (droga == null)
                {
                    throw new ArgumentNullException(nameof(droga));
                }

                droga.FechaBaja = DateTime.Now;
                droga.Activo = false;
                _context.Drogas.Update(droga);
                await _context.SaveChangesAsync();
                return droga;
            }
            catch (DbUpdateException ex)
            {
                // Manejo de excepciones
                throw new Exception("Error al eliminar la droga", ex);
            }
        }

        public async Task<Droga> ObtenerDrogaPorIdAsync(int id)
        {
          return await _context.Drogas.FindAsync(id);
            
        }

        public async Task<List<Droga>> ObtenerDrogasAsync()
        {
            return await _context.Drogas.ToListAsync();
        }

             

        public async Task<List<Droga>> ObtenerDrogasPorNombreAsync(string nombre)
        {
            return await _context.Drogas
          .Where(d => d.Nombre.Contains(nombre) && d.Activo)
          .ToListAsync();   
        }

        public async Task<List<Droga>> ObtenerDrogasPorRequiereRecetaAsync(bool requiereReceta)
        {
            return await _context.Drogas
                .Where(d => d.RequiereReceta == requiereReceta && d.Activo)
                .ToListAsync();
        }

        public async Task<List<Droga>> ObtenerDrogasActivasAsync()
        {
            return await _context.Drogas
                .Where(d => d.Activo)
                .ToListAsync();
        }


    }
}
