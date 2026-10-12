using SoporteTecnico.BL;
using SoporteTecnico.EN;
using SoporteTecnico.Tests.Auxiliares;

namespace SoporteTecnico.Tests.Servicios
{
    /// <summary>
    /// Validaciones de UsuarioBL que se resuelven antes de llegar al DAL.
    /// </summary>
    [TestClass]
    public class UsuarioBLTests
    {
        private const string PasswordValida = "Clave1234";
        private readonly UsuarioBL _bl = new();

        // ---------- RegistrarAsync ----------

        [TestMethod]
        public async Task Registrar_UsuarioNulo_Rechaza()
        {
            await AfirmarRegla.ErrorAsync(() => _bl.RegistrarAsync(null!, PasswordValida), "no puede ser nulo");
        }

        [TestMethod]
        [DataRow((string?)null)]
        [DataRow("")]
        [DataRow("   ")]
        public async Task Registrar_NombreVacio_Rechaza(string? pNombre)
        {
            var usuario = Datos.UsuarioValido();
            usuario.Nombre = pNombre!;

            await AfirmarRegla.ErrorAsync(() => _bl.RegistrarAsync(usuario, PasswordValida), "nombre es obligatorio");
        }

        [TestMethod]
        public async Task Registrar_NombreExcedeLongitud_Rechaza()
        {
            var usuario = Datos.UsuarioValido();
            usuario.Nombre = Datos.Texto(LongitudesCampo.UsuarioNombre + 1);

            await AfirmarRegla.ErrorAsync(
                () => _bl.RegistrarAsync(usuario, PasswordValida),
                $"{LongitudesCampo.UsuarioNombre} caracteres");
        }

        [TestMethod]
        [DataRow((string?)null)]
        [DataRow("")]
        [DataRow("   ")]
        public async Task Registrar_CorreoVacio_Rechaza(string? pCorreo)
        {
            var usuario = Datos.UsuarioValido();
            usuario.Correo = pCorreo!;

            await AfirmarRegla.ErrorAsync(() => _bl.RegistrarAsync(usuario, PasswordValida), "correo es obligatorio");
        }

        [TestMethod]
        public async Task Registrar_CorreoExcedeLongitud_Rechaza()
        {
            var usuario = Datos.UsuarioValido();
            usuario.Correo = Datos.Texto(LongitudesCampo.UsuarioCorreo + 1);

            await AfirmarRegla.ErrorAsync(
                () => _bl.RegistrarAsync(usuario, PasswordValida),
                $"{LongitudesCampo.UsuarioCorreo} caracteres");
        }

        [TestMethod]
        [DataRow("correo-sin-arroba")]
        [DataRow("usuario@")]
        [DataRow("@dominio.com")]
        [DataRow("Ana <ana@soporte.test>")]   // MailAddress lo acepta, pero Address != texto original
        public async Task Registrar_CorreoConFormatoInvalido_Rechaza(string pCorreo)
        {
            var usuario = Datos.UsuarioValido();
            usuario.Correo = pCorreo;

            await AfirmarRegla.ErrorAsync(() => _bl.RegistrarAsync(usuario, PasswordValida), "formato del correo");
        }

        [TestMethod]
        [DataRow(0)]
        [DataRow(-1)]
        public async Task Registrar_RolNoSeleccionado_Rechaza(int pIdRol)
        {
            var usuario = Datos.UsuarioValido();
            usuario.IdRol = pIdRol;

            await AfirmarRegla.ErrorAsync(() => _bl.RegistrarAsync(usuario, PasswordValida), "seleccionar un rol");
        }

        [TestMethod]
        [DataRow((string?)null)]
        [DataRow("")]
        public async Task Registrar_PasswordVacia_Rechaza(string? pPassword)
        {
            await AfirmarRegla.ErrorAsync(
                () => _bl.RegistrarAsync(Datos.UsuarioValido(), pPassword!),
                "contraseña es obligatoria");
        }

        [TestMethod]
        public async Task Registrar_PasswordCorta_Rechaza()
        {
            await AfirmarRegla.ErrorAsync(
                () => _bl.RegistrarAsync(Datos.UsuarioValido(), "1234567"),
                "al menos 8 caracteres");
        }

        [TestMethod]
        public async Task Registrar_DatosDeUsuarioInvalidosSeValidanAntesQueLaPassword()
        {
            var usuario = Datos.UsuarioValido();
            usuario.Nombre = string.Empty;

            await AfirmarRegla.ErrorAsync(() => _bl.RegistrarAsync(usuario, "123"), "nombre es obligatorio");
        }

        // ---------- IniciarSesionAsync ----------

        [TestMethod]
        [DataRow((string?)null, PasswordValida)]
        [DataRow("", PasswordValida)]
        [DataRow("   ", PasswordValida)]
        [DataRow("ana@soporte.test", null)]
        [DataRow("ana@soporte.test", "")]
        public async Task IniciarSesion_CredencialesIncompletas_Rechaza(string? pCorreo, string? pPassword)
        {
            await AfirmarRegla.ErrorAsync(
                () => _bl.IniciarSesionAsync(pCorreo!, pPassword!),
                "correo y la contraseña");
        }

        // ---------- ObtenerPorCorreoAsync ----------

        [TestMethod]
        [DataRow((string?)null)]
        [DataRow("")]
        [DataRow("   ")]
        public async Task ObtenerPorCorreo_CorreoVacio_Rechaza(string? pCorreo)
        {
            await AfirmarRegla.ErrorAsync(() => _bl.ObtenerPorCorreoAsync(pCorreo!), "correo es obligatorio");
        }

        // ---------- ObtenerPorIdAsync ----------

        [TestMethod]
        [DataRow(0)]
        [DataRow(-5)]
        public async Task ObtenerPorId_IdInvalido_Rechaza(int pId)
        {
            await AfirmarRegla.ErrorAsync(() => _bl.ObtenerPorIdAsync(pId), "identificador del usuario");
        }

        // ---------- ModificarAsync ----------

        [TestMethod]
        public async Task Modificar_UsuarioNulo_Rechaza()
        {
            await AfirmarRegla.ErrorAsync(() => _bl.ModificarAsync(null!), "no puede ser nulo");
        }

        [TestMethod]
        public async Task Modificar_NombreVacio_Rechaza()
        {
            var usuario = Datos.UsuarioValido();
            usuario.IdUsuario = 1;
            usuario.Nombre = "  ";

            await AfirmarRegla.ErrorAsync(() => _bl.ModificarAsync(usuario), "nombre es obligatorio");
        }

        [TestMethod]
        public async Task Modificar_CorreoConFormatoInvalido_Rechaza()
        {
            var usuario = Datos.UsuarioValido();
            usuario.IdUsuario = 1;
            usuario.Correo = "correo-sin-arroba";

            await AfirmarRegla.ErrorAsync(() => _bl.ModificarAsync(usuario), "formato del correo");
        }

        [TestMethod]
        public async Task Modificar_RolNoSeleccionado_Rechaza()
        {
            var usuario = Datos.UsuarioValido();
            usuario.IdUsuario = 1;
            usuario.IdRol = 0;

            await AfirmarRegla.ErrorAsync(() => _bl.ModificarAsync(usuario), "seleccionar un rol");
        }

        // ---------- CambiarEstadoAsync (activar / desactivar) ----------

        [TestMethod]
        [DataRow(0, true)]
        [DataRow(-1, false)]
        public async Task CambiarEstado_IdInvalido_Rechaza(int pId, bool pActivo)
        {
            await AfirmarRegla.ErrorAsync(() => _bl.CambiarEstadoAsync(pId, pActivo), "identificador del usuario");
        }
    }
}