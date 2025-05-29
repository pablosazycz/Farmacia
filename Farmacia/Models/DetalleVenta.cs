using System.ComponentModel.DataAnnotations.Schema;

namespace Farmacia.Models
{
    public class DetalleVenta
    {
        public int Id { get; set; }

        public int VentaId { get; set; }

        [ForeignKey("VentaId")]
        public Venta Venta { get; set; }

        public int ProductoId { get; set; }

        [ForeignKey("ProductoId")]
        public Producto Producto { get; set; }

        public int Cantidad { get; set; }

        public decimal PrecioUnitario { get; set; }


        public decimal Subtotal => Cantidad * PrecioUnitario;
    }


}
