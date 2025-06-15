using Farmacia.Models;

namespace Farmacia.Interfaces
{
    public interface ITicketService
    {
        byte[] GenerarTicketVentaPdf(Venta venta, Cliente? cliente);
    }
}