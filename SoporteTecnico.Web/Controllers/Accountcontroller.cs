using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SoporteTecnico.BL;
using SoporteTecnico.BL.Excepciones;
using SoporteTecnico.EN.Enumeraciones;
using SoporteTecnico.Web.ViewModels.Account;

namespace SoporteTecnico.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly UsuarioBL _usuarioBL = new UsuarioBL();
        private readonly ILogger<AccountController> _logger;

        public AccountController(ILogger<AccountController> logger)
        {
            _logger = logger;
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            if (User.Identity?.IsAuthenticated == true)
                return RedirectToAction("Index", "Home");

            return View(new LoginViewModel { ReturnUrl = returnUrl });
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            try
            {
                var usuario = await _usuarioBL.IniciarSesionAsync(model.Correo, model.Password);

                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, usuario.IdUsuario.ToString()),
                    new Claim(ClaimTypes.Name, usuario.Nombre),
                    new Claim(ClaimTypes.Email, usuario.Correo),
                    new Claim(ClaimTypes.Role, ((RolUsuario)usuario.IdRol).ToString())
                };

                var identidad = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(identidad));

                if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
                    return LocalRedirect(model.ReturnUrl);

                return RedirectToAction("Index", "Home");
            }
            catch (ReglaNegocioException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(model);
            }
            catch (Exception ex)
            {
                // Error técnico (por ejemplo, la base de datos no responde): se registra y no se muestra el detalle.
                _logger.LogError(ex, "Error al iniciar sesión.");
                ModelState.AddModelError(string.Empty, "Ocurrió un error inesperado. Intente de nuevo más tarde.");
                return View(model);
            }
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction(nameof(Login));
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult AccesoDenegado()
        {
            return View();
        }
    }
}