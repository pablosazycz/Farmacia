using System.ComponentModel.DataAnnotations;

namespace Farmacia.Models
{
    public class Cliente
    {
        public int Id { get; set; }
        [Required]
        public string Nombre { get; set; }
        [Required]
        public string Apellido { get; set; }
        [Required]
        public string Dni { get; set; }
        public string Telefono { get; set; }
        public string Email { get; set; }
        public string Domicilio { get; set; }

        public ICollection<Venta> Ventas { get; set; }

    }

}
