using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Farmacia.Models
{
    public class Droga
    {
        public int Id { get; set; }
        [Required]
        public string Nombre { get; set; }
        [Required]
        public string Concentracion { get; set; }

        public int Stock { get; set; }

        public int StockMinimo { get; set; }

        public int StockMaximo { get; set; }

        public bool RequiereReceta { get; set; }

        public bool Activo { get; set; }

        public DateTime? FechaAlta { get; set; }

        public DateTime? FechaBaja { get; set; }

        public ICollection<Producto> Productos { get; set; } = new List<Producto>();

        public ICollection<MovimientoStock> MovimientosStock { get; set; } = new List<MovimientoStock>();


        [NotMapped] // No se guarda en la base de datos
        public string NombreCompleto => $"{Nombre} {Concentracion}";

    }
}
