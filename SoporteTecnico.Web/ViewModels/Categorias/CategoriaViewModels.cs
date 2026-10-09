using System.ComponentModel.DataAnnotations;
using SoporteTecnico.EN;

namespace SoporteTecnico.Web.ViewModels.Categorias
{
    // Sirve para Create y Edit
    public class CategoriaFormViewModel
    {
        public int IdCategoria { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [StringLength(LongitudesCampo.CategoriaNombre, ErrorMessage = "El nombre no puede superar {1} caracteres.")]
        [Display(Name = "Nombre")]
        public string Nombre { get; set; } = string.Empty;

        [StringLength(LongitudesCampo.CategoriaDescripcion, ErrorMessage = "La descripción no puede superar {1} caracteres.")]
        [Display(Name = "Descripción")]
        public string? Descripcion { get; set; }
    }
}