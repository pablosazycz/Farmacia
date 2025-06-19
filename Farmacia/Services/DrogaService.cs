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
            {
                bool existe = await _context.Drogas.AnyAsync(d =>
                                d.Nombre.ToLower() == droga.Nombre.ToLower() &&
                                d.Concentracion.ToLower() == droga.Concentracion.ToLower() &&
                                d.Activo);

                if (existe)
                {
                    throw new InvalidOperationException("Ya existe una droga activa con ese nombre y concentración.");
                }
                droga.FechaAlta = DateTime.Now;
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

                bool existe = await _context.Drogas.AnyAsync(d =>
                               d.Nombre.ToLower() == droga.Nombre.ToLower() &&
                               d.Concentracion.ToLower() == droga.Concentracion.ToLower() &&
                               d.Activo);

                if (existe)
                {
                    throw new InvalidOperationException("Ya existe una droga activa con ese nombre y concentración.");
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

        public async Task<List<Droga>> BuscarDrogasAsync(string term)
        {
            if (string.IsNullOrWhiteSpace(term))
                return new List<Droga>();

            // Filtra en SQL solo por campos mapeados
            var query = _context.Drogas
                .Where(d => d.Nombre.Contains(term) && d.Activo)
                .OrderBy(d => d.Nombre)
                .Take(50); // Trae más para filtrar luego

            var drogas = await query.ToListAsync();

            // Ahora filtra en memoria por NombreCompleto
            return drogas
                .Where(d =>
                    d.Nombre.Contains(term, StringComparison.OrdinalIgnoreCase) ||
                    (!string.IsNullOrEmpty(d.NombreCompleto) && d.NombreCompleto.Contains(term, StringComparison.OrdinalIgnoreCase))
                )
                .Take(20)
                .ToList();
        }
    }
}
