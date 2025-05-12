using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Farmacia.Models
{
    public class Lote
    {
        public int Id { get; set; }
        [Required]
        public string CodigoLote { get; set; }
        [Required]
        public DateTime FechaVencimiento { get; set; }

        public int Cantidad { get; set; }

        public int ProductoId { get; set; }

        [ForeignKey("ProductoId")]
        public Producto Producto { get; set; }
    }

}
