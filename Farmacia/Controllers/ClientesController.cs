using Farmacia.Interfaces;
using Farmacia.Models;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Farmacia.Controllers
{
    public class ClientesController : Controller
    {
        private readonly IClienteService _clienteService;

        public ClientesController(IClienteService clienteService)
        {
            _clienteService = clienteService;
        }

        // GET: Clientes
        public async Task<IActionResult> Index()
        {
            var clientes = await _clienteService.ObtenerClientesAsync();
            return View(clientes);
        }

        // GET: Clientes/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var cliente = await _clienteService.ObtenerClientePorIdAsync(id);
            if (cliente == null)
                return NotFound();

            var historialPuntos = await _clienteService.ObtenerHistorialPuntosAsync(id);
            ViewBag.HistorialPuntos = historialPuntos;


            return View(cliente);
        }

        // GET: Clientes/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Clientes/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Cliente cliente)
        {
            if (ModelState.IsValid)
            {
                await _clienteService.CrearClienteAsync(cliente);
                return RedirectToAction(nameof(Index));
            }
            return View(cliente);
        }

        // GET: Clientes/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var cliente = await _clienteService.ObtenerClientePorIdAsync(id);
            if (cliente == null)
                return NotFound();
            return View(cliente);
        }

        // POST: Clientes/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Cliente cliente)
        {
            if (id != cliente.Id)
                return BadRequest();

            if (ModelState.IsValid)
            {
                await _clienteService.EditarClienteAsync(cliente);
                return RedirectToAction(nameof(Index));
            }
            return View(cliente);
        }

        // GET: Clientes/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var cliente = await _clienteService.ObtenerClientePorIdAsync(id);
            if (cliente == null)
                return NotFound();
            return View(cliente);
        }

        // POST: Clientes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _clienteService.EliminarClienteAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
