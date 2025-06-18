using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Farmacia.Models
{
    public class ClientePromocion
    {

        public int Id { get; set; }

        [Required]
        public int ClienteId { get; set; }

        [ForeignKey("ClienteId")]
        public Cliente Cliente { get; set; }

        [Required]
        public int PromocionId { get; set; }

        [ForeignKey("PromocionId")]
        public Promocion Promocion { get; set; }

        public DateTime FechaAplicacion { get; set; }

        public int? VentaId { get; set; }

        [ForeignKey("VentaId")]
        public Venta? Venta { get; set; }
    }
}
