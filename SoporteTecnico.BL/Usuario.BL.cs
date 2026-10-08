using System;
using System.Net.Mail;
using System.Threading.Tasks;
using SoporteTecnico.BL.Excepciones;
using SoporteTecnico.BL.Seguridad;
using SoporteTecnico.DAL;
using SoporteTecnico.EN.Entidades;

namespace SoporteTecnico.BL
{
    public class UsuarioBL
    {
        private const int NombreMaxLength = 100;
        private const int CorreoMaxLength = 150;
        private const int PasswordMinLength = 8;

        public async Task<int> RegistrarAsync(Usuario pUsuario, string pPassword)
        {
            Validar(pUsuario);
            ValidarPassword(pPassword);
            Normalizar(pUsuario);

            var existente = await UsuarioDAL.ObtenerPorCorreoAsync(pUsuario.Correo);
            if (existente != null)
                throw new ReglaNegocioException($"Ya existe un usuario registrado con el correo '{pUsuario.Correo}'.");

            pUsuario.PasswordHash = PasswordHasher.Hashear(pPassword);
            pUsuario.Activo = true;
            return await UsuarioDAL.GuardarAsync(pUsuario);
        }

        /// <summary>
        /// Valida las credenciales y devuelve el usuario con su rol (sin el hash de la contraseña).
        /// </summary>
        public async Task<Usuario> IniciarSesionAsync(string pCorreo, string pPassword)
        {
            if (string.IsNullOrWhiteSpace(pCorreo) || string.IsNullOrEmpty(pPassword))
                throw new ReglaNegocioException("Debe ingresar el correo y la contraseña.");

            var usuario = await UsuarioDAL.ObtenerPorCorreoAsync(pCorreo.Trim().ToLowerInvariant());

            // Mismo mensaje para "no existe" y "contraseña incorrecta" para no revelar qué correos están registrados.
            if (usuario == null || !PasswordHasher.Verificar(pPassword, usuario.PasswordHash))
                throw new ReglaNegocioException("Correo o contraseña incorrectos.");

            if (!usuario.Activo)
                throw new ReglaNegocioException("El usuario está inactivo. Comuníquese con un administrador.");

            usuario.PasswordHash = string.Empty;
            return usuario;
        }

        public async Task<Usuario?> ObtenerPorCorreoAsync(string pCorreo)
        {
            if (string.IsNullOrWhiteSpace(pCorreo))
                throw new ReglaNegocioException("El correo es obligatorio.");

            var usuario = await UsuarioDAL.ObtenerPorCorreoAsync(pCorreo.Trim().ToLowerInvariant());
            if (usuario != null)
                usuario.PasswordHash = string.Empty;

            return usuario;
        }

        // ---------- Reglas de negocio ----------

        private static void Validar(Usuario pUsuario)
        {
            if (pUsuario == null)
                throw new ReglaNegocioException("El usuario no puede ser nulo.");

            if (string.IsNullOrWhiteSpace(pUsuario.Nombre))
                throw new ReglaNegocioException("El nombre es obligatorio.");

            if (pUsuario.Nombre.Trim().Length > NombreMaxLength)
                throw new ReglaNegocioException($"El nombre no puede superar los {NombreMaxLength} caracteres.");

            if (string.IsNullOrWhiteSpace(pUsuario.Correo))
                throw new ReglaNegocioException("El correo es obligatorio.");

            if (pUsuario.Correo.Trim().Length > CorreoMaxLength)
                throw new ReglaNegocioException($"El correo no puede superar los {CorreoMaxLength} caracteres.");

            if (!EsCorreoValido(pUsuario.Correo.Trim()))
                throw new ReglaNegocioException("El formato del correo no es válido.");

            if (pUsuario.IdRol <= 0)
                throw new ReglaNegocioException("Debe seleccionar un rol para el usuario.");
        }

        private static void ValidarPassword(string pPassword)
        {
            if (string.IsNullOrEmpty(pPassword))
                throw new ReglaNegocioException("La contraseña es obligatoria.");

            if (pPassword.Length < PasswordMinLength)
                throw new ReglaNegocioException($"La contraseña debe tener al menos {PasswordMinLength} caracteres.");
        }

        private static void Normalizar(Usuario pUsuario)
        {
            pUsuario.Nombre = pUsuario.Nombre.Trim();
            pUsuario.Correo = pUsuario.Correo.Trim().ToLowerInvariant();
        }

        private static bool EsCorreoValido(string pCorreo)
        {
            try
            {
                var direccion = new MailAddress(pCorreo);
                return direccion.Address == pCorreo;
            }
            catch (FormatException)
            {
                return false;
            }
        }
    }
}