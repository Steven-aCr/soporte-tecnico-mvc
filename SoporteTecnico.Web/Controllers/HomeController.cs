using System.Diagnostics;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SoporteTecnico.BL;
using SoporteTecnico.BL.Excepciones;
using SoporteTecnico.EN.Enumeraciones;
using SoporteTecnico.Web.Models;
using SoporteTecnico.Web.ViewModels.Home;

namespace SoporteTecnico.Web.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly TicketBL _ticketBL = new TicketBL();
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            var model = new HomeIndexViewModel();

            try
            {
                var idActor = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
                var tickets = await _ticketBL.ListarAsync(idActor);

                model.Pendientes = tickets.Count(t => t.Estado == EstadoTicket.Pendiente);
                model.EnProceso = tickets.Count(t => t.Estado == EstadoTicket.EnProceso);
                model.Resueltos = tickets.Count(t => t.Estado == EstadoTicket.Resuelto);
                model.Cerrados = tickets.Count(t => t.Estado == EstadoTicket.Cerrado);

                model.Recientes = tickets
                    .OrderByDescending(t => t.FechaCreacion)
                    .Take(5)
                    .ToList();
            }
            catch (ReglaNegocioException ex)
            {
                TempData["Error"] = ex.Message;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cargar el resumen de inicio.");
                TempData["Error"] = "No se pudo cargar el resumen.";
            }

            return View(model);
        }

        [AllowAnonymous]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}