using Farmacia.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Farmacia.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }


        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<Producto> Productos { get; set; }
        public DbSet<Venta> Ventas { get; set; }
        public DbSet<DetalleVenta> DetallesVenta { get; set; }
        public DbSet<Droga> Drogas { get; set; }
        public DbSet<Lote> Lotes { get; set; }
        public DbSet<Promocion> Promociones { get; set; }
        public DbSet<ClientePromocion> ClientePromociones { get; set; }
        public DbSet<MovimientoStock> MovimientosStock { get; set; }
        public DbSet<ClientePunto> ClientePuntos { get; set; }

    }
}
