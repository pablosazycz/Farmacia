using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Farmacia.Models
{
    public class ClientePunto
    {
        public int Id { get; set; }

        [Required]
        public int ClienteId { get; set; }
        [ForeignKey("ClienteId")]
        public Cliente Cliente { get; set; }

        public int Puntos { get; set; } // Positivo (acumula), negativo (canjea)

        [Required]
        public DateTime Fecha { get; set; } = DateTime.Now;

        public string Motivo { get; set; } // Ej: "Compra", "Canje", "Promo", etc.

        public int? PromocionId { get; set; }
        [ForeignKey("PromocionId")]
        public Promocion? Promocion { get; set; }

        public int? VentaId { get; set; }
        [ForeignKey("VentaId")]
        public Venta? Venta { get; set; }

    }
}
