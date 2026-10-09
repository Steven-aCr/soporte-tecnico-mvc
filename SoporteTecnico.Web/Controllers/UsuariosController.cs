using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SoporteTecnico.BL;
using SoporteTecnico.BL.Excepciones;
using SoporteTecnico.EN.Entidades;
using SoporteTecnico.Web.ViewModels.Usuarios;

namespace SoporteTecnico.Web.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class UsuariosController : Controller
    {
        private readonly UsuarioBL _usuarioBL = new UsuarioBL();
        private readonly ILogger<UsuariosController> _logger;

        public UsuariosController(ILogger<UsuariosController> logger)
        {
            _logger = logger;
        }

        private int IdActor => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            try
            {
                var usuarios = await _usuarioBL.ObtenerTodosAsync();
                return View(usuarios);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al listar los usuarios.");
                TempData["Error"] = "No se pudieron cargar los usuarios.";
                return View(new List<Usuario>());
            }
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            return View(new UsuarioCreateViewModel { Roles = await CargarRolesAsync() });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UsuarioCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.Roles = await CargarRolesAsync();
                return View(model);
            }

            try
            {
                await _usuarioBL.RegistrarAsync(new Usuario
                {
                    Nombre = model.Nombre,
                    Correo = model.Correo,
                    IdRol = model.IdRol
                }, model.Password);

                TempData["Exito"] = "Usuario creado correctamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (ReglaNegocioException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear el usuario.");
                ModelState.AddModelError(string.Empty, "Ocurrió un error inesperado al guardar el usuario.");
            }

            model.Roles = await CargarRolesAsync();
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var usuario = await _usuarioBL.ObtenerPorIdAsync(id);

                return View(new UsuarioEditViewModel
                {
                    IdUsuario = usuario.IdUsuario,
                    Nombre = usuario.Nombre,
                    Correo = usuario.Correo,
                    IdRol = usuario.IdRol,
                    Roles = await CargarRolesAsync()
                });
            }
            catch (ReglaNegocioException ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cargar el usuario {IdUsuario}.", id);
                TempData["Error"] = "No se pudo cargar el usuario.";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, UsuarioEditViewModel model)
        {
            if (id != model.IdUsuario)
                return BadRequest();

            if (!ModelState.IsValid)
            {
                model.Roles = await CargarRolesAsync();
                return View(model);
            }

            try
            {
                await _usuarioBL.ModificarAsync(new Usuario
                {
                    IdUsuario = model.IdUsuario,
                    Nombre = model.Nombre,
                    Correo = model.Correo,
                    IdRol = model.IdRol
                });

                TempData["Exito"] = "Usuario actualizado correctamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (ReglaNegocioException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar el usuario {IdUsuario}.", id);
                ModelState.AddModelError(string.Empty, "Ocurrió un error inesperado al guardar los cambios.");
            }

            model.Roles = await CargarRolesAsync();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Desactivar(int id)
        {
            if (id == IdActor)
            {
                TempData["Error"] = "No puedes desactivar tu propio usuario.";
                return RedirectToAction(nameof(Index));
            }

            return await EjecutarAsync(() => _usuarioBL.CambiarEstadoAsync(id, false), "Usuario desactivado.");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public Task<IActionResult> Reactivar(int id) =>
            EjecutarAsync(() => _usuarioBL.CambiarEstadoAsync(id, true), "Usuario reactivado.");

        private async Task<List<SelectListItem>> CargarRolesAsync()
        {
            var roles = await _usuarioBL.ObtenerRolesAsync();
            return roles.Select(r => new SelectListItem(r.Nombre, r.IdRol.ToString())).ToList();
        }

        private async Task<IActionResult> EjecutarAsync(Func<Task> accion, string mensajeExito)
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
                _logger.LogError(ex, "Error en una acción de usuarios.");
                TempData["Error"] = "Ocurrió un error inesperado. Intente de nuevo.";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}