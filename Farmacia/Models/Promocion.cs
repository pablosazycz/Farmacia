namespace Farmacia.Models
{
    public class Promocion
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public decimal Descuento { get; set; } // Ej: 0.10 para 10%
        public string Tipo { get; set; } // Ej: "PorCantidadDeCompras", "Manual"
        public int? CantidadMinima { get; set; } // Opcional, según el tipo
        public string? Periodo { get; set; } // Ej: "Mensual"
        public bool Activa { get; set; }

        public ICollection<ClientePromocion>? ClientePromociones { get; set; }
    }

}
