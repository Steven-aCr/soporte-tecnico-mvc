using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using SoporteTecnico.EN;

namespace SoporteTecnico.Web.ViewModels.Usuarios
{
    public class UsuarioCreateViewModel
    {
        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [StringLength(LongitudesCampo.UsuarioNombre, ErrorMessage = "El nombre no puede superar {1} caracteres.")]
        [Display(Name = "Nombre")]
        public string Nombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "El correo es obligatorio.")]
        [EmailAddress(ErrorMessage = "El formato del correo no es válido.")]
        [StringLength(LongitudesCampo.UsuarioCorreo, ErrorMessage = "El correo no puede superar {1} caracteres.")]
        [Display(Name = "Correo electrónico")]
        public string Correo { get; set; } = string.Empty;

        [Required(ErrorMessage = "La contraseña es obligatoria.")]
        [MinLength(8, ErrorMessage = "La contraseña debe tener al menos 8 caracteres.")]
        [DataType(DataType.Password)]
        [Display(Name = "Contraseña")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Confirme la contraseña.")]
        [Compare(nameof(Password), ErrorMessage = "Las contraseñas no coinciden.")]
        [DataType(DataType.Password)]
        [Display(Name = "Confirmar contraseña")]
        public string ConfirmarPassword { get; set; } = string.Empty;

        [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar un rol.")]
        [Display(Name = "Rol")]
        public int IdRol { get; set; }

        public List<SelectListItem> Roles { get; set; } = new();
    }

    public class UsuarioEditViewModel
    {
        public int IdUsuario { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [StringLength(LongitudesCampo.UsuarioNombre, ErrorMessage = "El nombre no puede superar {1} caracteres.")]
        [Display(Name = "Nombre")]
        public string Nombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "El correo es obligatorio.")]
        [EmailAddress(ErrorMessage = "El formato del correo no es válido.")]
        [StringLength(LongitudesCampo.UsuarioCorreo, ErrorMessage = "El correo no puede superar {1} caracteres.")]
        [Display(Name = "Correo electrónico")]
        public string Correo { get; set; } = string.Empty;

        [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar un rol.")]
        [Display(Name = "Rol")]
        public int IdRol { get; set; }

        public List<SelectListItem> Roles { get; set; } = new();
    }
}