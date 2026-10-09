using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SoporteTecnico.BL;
using SoporteTecnico.BL.Excepciones;
using SoporteTecnico.EN.Entidades;
using SoporteTecnico.Web.ViewModels.Categorias;

namespace SoporteTecnico.Web.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class CategoriasController : Controller
    {
        private readonly CategoriaBL _categoriaBL = new CategoriaBL();
        private readonly ILogger<CategoriasController> _logger;

        public CategoriasController(ILogger<CategoriasController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            try
            {
                var categorias = await _categoriaBL.ObtenerTodosAsync();
                return View(categorias);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al listar las categorías.");
                TempData["Error"] = "No se pudieron cargar las categorías.";
                return View(new List<Categoria>());
            }
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View(new CategoriaFormViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CategoriaFormViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            try
            {
                await _categoriaBL.GuardarAsync(new Categoria
                {
                    Nombre = model.Nombre,
                    Descripcion = model.Descripcion
                });

                TempData["Exito"] = "Categoría creada correctamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (ReglaNegocioException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear la categoría.");
                ModelState.AddModelError(string.Empty, "Ocurrió un error inesperado al guardar la categoría.");
                return View(model);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var categoria = await _categoriaBL.ObtenerPorIdAsync(new Categoria { IdCategoria = id });
                if (categoria == null)
                {
                    TempData["Error"] = "La categoría no existe.";
                    return RedirectToAction(nameof(Index));
                }

                return View(new CategoriaFormViewModel
                {
                    IdCategoria = categoria.IdCategoria,
                    Nombre = categoria.Nombre,
                    Descripcion = categoria.Descripcion
                });
            }
            catch (ReglaNegocioException ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cargar la categoría {IdCategoria}.", id);
                TempData["Error"] = "No se pudo cargar la categoría.";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CategoriaFormViewModel model)
        {
            if (id != model.IdCategoria)
                return BadRequest();

            if (!ModelState.IsValid)
                return View(model);

            try
            {
                // Se parte de la categoría existente para conservar su estado (Activo).
                var categoria = await _categoriaBL.ObtenerPorIdAsync(new Categoria { IdCategoria = id });
                if (categoria == null)
                {
                    TempData["Error"] = "La categoría no existe.";
                    return RedirectToAction(nameof(Index));
                }

                categoria.Nombre = model.Nombre;
                categoria.Descripcion = model.Descripcion;
                await _categoriaBL.ModificarAsync(categoria);

                TempData["Exito"] = "Categoría actualizada correctamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (ReglaNegocioException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar la categoría {IdCategoria}.", id);
                ModelState.AddModelError(string.Empty, "Ocurrió un error inesperado al guardar los cambios.");
                return View(model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public Task<IActionResult> Desactivar(int id) =>
            EjecutarAsync(() => _categoriaBL.EliminarAsync(new Categoria { IdCategoria = id }),
                          "Categoría desactivada.");

        [HttpPost]
        [ValidateAntiForgeryToken]
        public Task<IActionResult> Reactivar(int id) =>
            EjecutarAsync(() => _categoriaBL.ReactivarAsync(id), "Categoría reactivada.");

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
                _logger.LogError(ex, "Error en una acción de categorías.");
                TempData["Error"] = "Ocurrió un error inesperado. Intente de nuevo.";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}