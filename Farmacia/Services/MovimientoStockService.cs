using Farmacia.Data;
using Farmacia.Interfaces;
using Farmacia.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Farmacia.Services
{
    public class MovimientoStockService : IMovimientoStockService
    {
        private readonly IDrogaService _drogaService;
        private readonly IProductoService _productoService;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _UserManager;

        public MovimientoStockService(
            IDrogaService drogaService,
            IProductoService productoService,
            ApplicationDbContext context,
            UserManager<IdentityUser> userManager)
        {
            _drogaService = drogaService;
            _productoService = productoService;
            _context = context;
            userManager = _UserManager;

        }

        //public async Task<MovimientoStock> CrearMovimientoAsync(MovimientoStock movimiento)
        //{


        //    Droga droga = await _drogaService.ObtenerDrogaPorIdAsync(movimiento.DrogaId);
        //    if (droga == null)
        //        throw new ArgumentException("La droga especificada no existe.");
        //    Producto producto = await _productoService.ObtenerProductoPorIdAsync(movimiento.ProductoId);
        //    if (producto == null)
        //        throw new ArgumentException("La droga o el producto no existen.");

        //    if (movimiento.Fecha == default)
        //        movimiento.Fecha = DateTime.Now;

        //    if (movimiento.Cantidad < 0)
        //        throw new ArgumentException("La cantidad no puede ser negativa.");

        //    if (string.IsNullOrWhiteSpace(movimiento.Lote.CodigoLote))
        //    {
        //        throw new ArgumentException("El código de lote es obligatorio.");
        //    }

        //    Lote loteExistente = await _context.Lotes
        //        .FirstOrDefaultAsync(l => l.CodigoLote == movimiento.CodigoLote && l.ProductoId == movimiento.ProductoId);

        //    switch (movimiento.TipoMovimiento)
        //    {
        //        case TipoMovimiento.Compra:
        //            // Si el lote no existe, lo creamos
        //            if (loteExistente == null)
        //            {
        //                Lote nuevoLote = new Lote
        //                {
        //                    CodigoLote = movimiento.CodigoLote,
        //                    FechaVencimiento = movimiento.Fecha,
        //                    Cantidad = movimiento.Cantidad,
        //                    ProductoId = (int)movimiento.ProductoId
        //                };

        //                _context.Lotes.Add(nuevoLote);
        //                await _context.SaveChangesAsync();
        //                movimiento.Lote.Id = nuevoLote.Id;
        //                movimiento.Lote = nuevoLote;

        //                droga.Stock += movimiento.Cantidad;
        //                _context.Drogas.Update(droga);
        //                await _context.SaveChangesAsync();
        //            }
        //            else
        //            {
        //                // Si el lote ya existe, actualizamos la cantidad
        //                loteExistente.Cantidad += movimiento.Cantidad;
        //                _context.Lotes.Update(loteExistente);
        //                await _context.SaveChangesAsync();
        //                movimiento.Lote.Id = loteExistente.Id;
        //                movimiento.Lote = loteExistente;

        //                droga.Stock += movimiento.Cantidad;
        //                _context.Drogas.Update(droga);
        //                await _context.SaveChangesAsync();
        //            }

        //            break;
        //        case TipoMovimiento.Venta:
        //            if (loteExistente != null)
        //            {
        //                if (loteExistente.Cantidad >= movimiento.Cantidad)
        //                {
        //                    loteExistente.Cantidad -= movimiento.Cantidad;
        //                    _context.Lotes.Update(loteExistente);
        //                    await _context.SaveChangesAsync();
        //                    movimiento.Lote.Id = loteExistente.Id;
        //                    movimiento.Lote = loteExistente;

        //                    droga.Stock -= movimiento.Cantidad;
        //                    _context.Drogas.Update(droga);
        //                    await _context.SaveChangesAsync();

        //                }
        //                else
        //                {
        //                    throw new ArgumentException("No hay suficiente cantidad en el lote para realizar la venta.");
        //                }

        //            }

        //            else
        //            {
        //                throw new ArgumentException("El lote especificado no existe para la venta.");
        //            }
        //            break;
        //    }

        //    _context.MovimientosStock.Add(movimiento);
        //    await _context.SaveChangesAsync();

        //    return movimiento;
        //}

        public async Task<MovimientoStock> CrearMovimientoAsync(MovimientoStock movimiento)
        {
            // Validaciones iniciales
            var droga = await _drogaService.ObtenerDrogaPorIdAsync(movimiento.DrogaId);
            if (droga == null)
                throw new ArgumentException("La droga especificada no existe.");

            var producto = await _productoService.ObtenerProductoPorIdAsync(movimiento.ProductoId);
            if (producto == null)
                throw new ArgumentException("El producto especificado no existe.");

            // Validar que el producto pertenece a la droga
            if (producto.DrogaId != movimiento.DrogaId)
                throw new ArgumentException("El producto no pertenece a la droga seleccionada.");

            if (movimiento.Fecha == default)
                movimiento.Fecha = DateTime.Now;

            if (movimiento.Cantidad < 0)
                throw new ArgumentException("La cantidad no puede ser negativa.");

            if (string.IsNullOrWhiteSpace(movimiento.CodigoLote))
                throw new ArgumentException("El código de lote es obligatorio.");

            // Buscar lote existente
            var loteExistente = await _context.Lotes
                .FirstOrDefaultAsync(l => l.CodigoLote == movimiento.CodigoLote && l.ProductoId == movimiento.ProductoId);

            switch (movimiento.TipoMovimiento)
            {
                case TipoMovimiento.Compra:
                    if (loteExistente == null)
                    {
                        var nuevoLote = new Lote
                        {
                            CodigoLote = movimiento.CodigoLote,
                            FechaVencimiento = movimiento.FechaVencimiento,
                            Cantidad = movimiento.Cantidad,
                            ProductoId = (int)movimiento.ProductoId,
                            PrecioCompra=movimiento.Lote.PrecioCompra
                        };
                        _context.Lotes.Add(nuevoLote);
                        await _context.SaveChangesAsync(); // para obtener el Id

                        decimal porcentajeGanancia = 0.30m; // 30%
                        producto.PrecioUnitario = Math.Round(nuevoLote.PrecioCompra * (1 + porcentajeGanancia), 2);
                        if (movimiento.Lote.PrecioCompra <= 0)
                            throw new ArgumentException("El precio de compra debe ser mayor a cero.");
                        _context.Productos.Update(producto);
                        await _context.SaveChangesAsync();

                        movimiento.Lote = nuevoLote;
                        movimiento.LoteId = nuevoLote.Id;
                    }
                    else
                    {
                        loteExistente.Cantidad += movimiento.Cantidad;
                        _context.Lotes.Update(loteExistente);
                        movimiento.Lote = loteExistente;
                        movimiento.LoteId = loteExistente.Id;
                    }
                    int unidades = movimiento.Cantidad * producto.CantidadPresentacion;
                    droga.Stock +=unidades;
                    _context.Drogas.Update(droga);
                    break;

                case TipoMovimiento.Venta:
                    if (loteExistente == null)
                        throw new ArgumentException("El lote especificado no existe para la venta.");

                    if (loteExistente.Cantidad < movimiento.Cantidad)
                        throw new ArgumentException("No hay suficiente cantidad en el lote para realizar la venta.");

                    loteExistente.Cantidad -= movimiento.Cantidad;
                    _context.Lotes.Update(loteExistente);

                    movimiento.Lote = loteExistente;
                    movimiento.LoteId = loteExistente.Id;

                    int unidadesVendidas = movimiento.Cantidad * producto.CantidadPresentacion;
                    droga.Stock -= unidadesVendidas;
                    _context.Drogas.Update(droga);
                    break;

                case TipoMovimiento.BajaPorVencimiento:

                    if (loteExistente == null)
                        throw new ArgumentException("El lote especificado no existe para dar de baja por vencimiento.");
                    if (loteExistente.FechaVencimiento >= DateTime.Now)
                        throw new ArgumentException("El lote no está vencido.");
                    if (loteExistente.Cantidad < movimiento.Cantidad)
                        throw new ArgumentException("No hay suficiente cantidad en el lote para dar de baja por vencimiento.");
                    loteExistente.Cantidad -= movimiento.Cantidad;
                    _context.Lotes.Update(loteExistente);
                    movimiento.Lote = loteExistente;
                    movimiento.LoteId = loteExistente.Id;
                    int unidadesBaja = movimiento.Cantidad * producto.CantidadPresentacion;
                    droga.Stock -= unidadesBaja;
                    _context.Drogas.Update(droga);
                    break;

                default:
                    throw new ArgumentException("Tipo de movimiento inválido.");
            }

            _context.MovimientosStock.Add(movimiento);
            await _context.SaveChangesAsync();

            return movimiento;
        }
            

        public async Task<List<MovimientoStock>> ObtenerMovimientosPorDrogaIdAsync(int drogaId)
        {
            return await _context.MovimientosStock
                        .Where(m => m.DrogaId == drogaId)
                        .Include(m => m.Droga)
                        .Include(m => m.Lote)
                        .ToListAsync();
        }

        public async Task<List<MovimientoStock>> ObtenerMovimientosPorFechaAsync(DateTime fecha)
        {
            return await _context.MovimientosStock
                        .Where(m => m.Fecha.Date == fecha.Date)
                        .Include(m => m.Droga)
                        .Include(m => m.Lote)
                        .ToListAsync();
        }

        public async Task<List<MovimientoStock>> ObtenerMovimientosPorTipoAsync(TipoMovimiento tipoMovimiento)
        {
            return await _context.MovimientosStock
                          .Where(m => m.TipoMovimiento == tipoMovimiento)
                          .Include(m => m.Droga)
                          .Include(m => m.Lote)
                          .ToListAsync();
        }

        public async Task<List<MovimientoStock>> ObtenerTodosAsync()
        {
            return await _context.MovimientosStock
                        .Include(m => m.Droga)
                        .Include(m => m.Lote)
                        .Include(m=>m.Usuario)
                        .Include(m=>m.Producto)
                        .ToListAsync();
        }
    }
}
