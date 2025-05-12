using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Farmacia.Models
{
    public class MovimientoStock
    {
        public int Id { get; set; }

        public DateTime Fecha { get; set; }

        [Required]
        public int DrogaId { get; set; }

        [ForeignKey("DrogaId")]
        public Droga Droga { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "La cantidad no puede ser negativa.")]
        public int Cantidad { get; set; }

        [Required]
        public TipoMovimiento TipoMovimiento { get; set; }

        [StringLength(500, ErrorMessage = "Las observaciones no pueden superar los 500 caracteres.")]
        public string Observaciones { get; set; }

        [Required]
        public string UsuarioId { get; set; }

        [ForeignKey("UsuarioId")]
        public IdentityUser Usuario { get; set; }

        public int? ProductoId { get; set; }

        [ForeignKey("ProductoId")]
        public Producto Producto { get; set; }

        [Required]
        public string CodigoLote { get; set; }

     
        public Lote Lote { get; set; }
    }


    public enum TipoMovimiento
    {
        Compra,       // Cuando se recibe un nuevo lote de medicamentos
        Venta,        // Cuando se vende un medicamento
        Reposicion,   // Cuando se hace una reposición de stock
        Devolucion    // Cuando un cliente devuelve un medicamento
    }
}
