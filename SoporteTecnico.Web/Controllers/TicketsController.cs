using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SoporteTecnico.BL;
using SoporteTecnico.BL.Excepciones;
using SoporteTecnico.EN.Entidades;
using SoporteTecnico.EN.Enumeraciones;
using SoporteTecnico.Web.ViewModels.Tickets;

namespace SoporteTecnico.Web.Controllers
{
    [Authorize]
    public class TicketsController : Controller
    {
        private readonly TicketBL _ticketBL = new TicketBL();
        private readonly ComentarioBL _comentarioBL = new ComentarioBL();
        private readonly CategoriaBL _categoriaBL = new CategoriaBL();
        private readonly UsuarioBL _usuarioBL = new UsuarioBL();
        private readonly ILogger<TicketsController> _logger;

        public TicketsController(ILogger<TicketsController> logger)
        {
            _logger = logger;
        }

        private int IdActor => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        private RolUsuario RolActor => Enum.Parse<RolUsuario>(User.FindFirstValue(ClaimTypes.Role)!);

        [HttpGet]
        public async Task<IActionResult> Index(EstadoTicket? estado)
        {
            var model = new TicketIndexViewModel { Estado = estado };

            try
            {
                model.Tickets = await _ticketBL.ListarAsync(IdActor, estado);
            }
            catch (ReglaNegocioException ex)
            {
                TempData["Error"] = ex.Message;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al listar los tickets.");
                TempData["Error"] = "No se pudieron cargar los tickets.";
            }

            return View(model);
        }

        [Authorize(Roles = "Solicitante")]
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var model = new TicketCreateViewModel();
            await CargarCategoriasAsync(model);
            return View(model);
        }

        [Authorize(Roles = "Solicitante")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TicketCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                await CargarCategoriasAsync(model);
                return View(model);
            }

            try
            {
                var ticket = new Ticket
                {
                    Titulo = model.Titulo,
                    Descripcion = model.Descripcion,
                    Prioridad = model.Prioridad,
                    IdCategoria = model.IdCategoria,
                    IdSolicitante = IdActor   // siempre el usuario autenticado
                };

                await _ticketBL.GuardarAsync(ticket);

                TempData["Exito"] = "Ticket creado correctamente.";
                return RedirectToAction(nameof(Details), new { id = ticket.IdTicket });
            }
            catch (ReglaNegocioException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear el ticket.");
                ModelState.AddModelError(string.Empty, "Ocurrió un error inesperado al crear el ticket.");
            }

            await CargarCategoriasAsync(model);
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var model = new TicketDetailsViewModel
                {
                    Ticket = await _ticketBL.ObtenerDetalleAsync(IdActor, id),
                    Comentarios = await _comentarioBL.ObtenerPorTicketAsync(IdActor, id),
                    Historial = await _ticketBL.ObtenerHistorialAsync(IdActor, id),
                    IdUsuarioActual = IdActor,
                    Rol = RolActor
                };

                if (model.PuedeAsignar)
                    model.Tecnicos = await _usuarioBL.ObtenerTecnicosActivosAsync();

                return View(model);
            }
            catch (ReglaNegocioException ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cargar el ticket {IdTicket}.", id);
                TempData["Error"] = "No se pudo cargar el ticket.";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public Task<IActionResult> Asignar(int id, int idTecnico) =>
            EjecutarAsync(id, () => _ticketBL.AsignarAsync(IdActor, id, idTecnico),
                          "Técnico asignado correctamente.");

        [HttpPost]
        [ValidateAntiForgeryToken]
        public Task<IActionResult> Iniciar(int id) =>
            EjecutarAsync(id, () => _ticketBL.IniciarAsync(IdActor, id),
                          "El ticket está en proceso.");

        [HttpPost]
        [ValidateAntiForgeryToken]
        public Task<IActionResult> Resolver(int id, string solucion) =>
            EjecutarAsync(id, () => _ticketBL.ResolverAsync(IdActor, id, solucion),
                          "Ticket marcado como resuelto.");

        [HttpPost]
        [ValidateAntiForgeryToken]
        public Task<IActionResult> Cerrar(int id) =>
            EjecutarAsync(id, () => _ticketBL.CerrarAsync(IdActor, id),
                          "Ticket cerrado.");

        [HttpPost]
        [ValidateAntiForgeryToken]
        public Task<IActionResult> Comentar(int id, string contenido) =>
            EjecutarAsync(id, () => _comentarioBL.GuardarAsync(new Comentario
            {
                IdTicket = id,
                IdUsuario = IdActor,
                Contenido = contenido ?? string.Empty
            }), "Comentario agregado.");

        private async Task CargarCategoriasAsync(TicketCreateViewModel model)
        {
            var categorias = await _categoriaBL.ObtenerTodosAsync(null, true);
            model.Categorias = categorias
                .Select(c => new SelectListItem(c.Nombre, c.IdCategoria.ToString()))
                .ToList();
        }

        private async Task<IActionResult> EjecutarAsync(int id, Func<Task> accion, string mensajeExito)
        {
            try
            {
                await accion();
                TempData["Exito"] = mensajeExito;
            }
            catch (ReglaNegocioException ex)
            {
                TempData["Error"] = ex.Message;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en una acción sobre el ticket {IdTicket}.", id);
                TempData["Error"] = "Ocurrió un error inesperado. Intente de nuevo.";
            }

            return RedirectToAction(nameof(Details), new { id });
        }
    }
}