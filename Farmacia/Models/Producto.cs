using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Farmacia.Models
{
    public class Producto
    {
        public int Id { get; set; }

        [Required]
        [Display(Name ="Nombre")]
        public string NombreComercial { get; set; }
        [Display(Name = "Lab")]
        public string Laboratorio { get; set; }

        [Required]
        [Display(Name = "Form Farm")]
        public string FormaFarmaceutica { get; set; }

        [Required]
        [Display(Name = "Tipo de Presentación")]
        public TipoPresentacionEnum TipoPresentacion { get; set; }

        [Display(Name = "Presentacion")]
        public int CantidadPresentacion { get; set; }

        [Display(Name = "EAN")]
        public string CodigoBarras { get; set; }

        public string Estado { get; set; }

        public bool Activo { get; set; }

        public DateTime? FechaAlta { get; set; }

        public DateTime? FechaBaja { get; set; }

        [ForeignKey("Droga")]
        [Display(Name = "Generico")]
        public int DrogaId { get; set; }

        public Droga Droga { get; set; }

        public ICollection<Lote> Lotes { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal PrecioUnitario { get; set; }

        [NotMapped]
        public string PresentacionCompleta
        {
            get
            {
                return $"{TipoPresentacion.ToString()} X {CantidadPresentacion} {FormaFarmaceutica?.ToString()}(s)";
            }
        }

    }

    public enum TipoPresentacionEnum
    {
        Caja,
        Blister,
        Frasco,
        Sachet,
        Ampolla,
        Tubo
    }

    public enum EstadoEnum
    {
        Activo,
        Inactivo,
        Descontinuado
    }


}

//TODO: Agreegar en la presentacion si es caja blister o como viene para que quede mejor en el nombre