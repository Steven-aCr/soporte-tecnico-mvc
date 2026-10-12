using SoporteTecnico.BL;
using SoporteTecnico.EN;
using SoporteTecnico.EN.Entidades;
using SoporteTecnico.Tests.Auxiliares;

namespace SoporteTecnico.Tests.Servicios
{

    /// <summary>
    /// Validaciones de CategoriaBL que se resuelven antes de llegar al DAL.
    /// </summary>
    [TestClass]
    public class CategoriaBLTests
    {
        private readonly CategoriaBL _bl = new();

        // ---------- GuardarAsync ----------

        [TestMethod]
        public async Task Guardar_CategoriaNula_Rechaza()
        {
            await AfirmarRegla.ErrorAsync(() => _bl.GuardarAsync(null!), "no puede ser nula");
        }

        [TestMethod]
        [DataRow((string?)null)]
        [DataRow("")]
        [DataRow("   ")]
        public async Task Guardar_NombreVacio_Rechaza(string? pNombre)
        {
            var categoria = Datos.CategoriaValida();
            categoria.Nombre = pNombre!;

            await AfirmarRegla.ErrorAsync(() => _bl.GuardarAsync(categoria), "nombre de la categoría es obligatorio");
        }

        [TestMethod]
        public async Task Guardar_NombreExcedeLongitud_Rechaza()
        {
            var categoria = Datos.CategoriaValida();
            categoria.Nombre = Datos.Texto(LongitudesCampo.CategoriaNombre + 1);

            await AfirmarRegla.ErrorAsync(
                () => _bl.GuardarAsync(categoria),
                $"{LongitudesCampo.CategoriaNombre} caracteres");
        }

        [TestMethod]
        public async Task Guardar_DescripcionExcedeLongitud_Rechaza()
        {
            var categoria = Datos.CategoriaValida();
            categoria.Descripcion = Datos.Texto(LongitudesCampo.CategoriaDescripcion + 1);

            await AfirmarRegla.ErrorAsync(
                () => _bl.GuardarAsync(categoria),
                $"{LongitudesCampo.CategoriaDescripcion} caracteres");
        }

        // ---------- ModificarAsync ----------

        [TestMethod]
        public async Task Modificar_CategoriaNula_Rechaza()
        {
            await AfirmarRegla.ErrorAsync(() => _bl.ModificarAsync(null!), "no puede ser nula");
        }

        [TestMethod]
        [DataRow(0)]
        [DataRow(-1)]
        public async Task Modificar_IdInvalido_Rechaza(int pId)
        {
            var categoria = Datos.CategoriaValida();
            categoria.IdCategoria = pId;

            await AfirmarRegla.ErrorAsync(() => _bl.ModificarAsync(categoria), "identificador de la categoría");
        }

        [TestMethod]
        public async Task Modificar_NombreVacio_Rechaza()
        {
            var categoria = Datos.CategoriaValida();
            categoria.Nombre = "  ";

            await AfirmarRegla.ErrorAsync(() => _bl.ModificarAsync(categoria), "nombre de la categoría es obligatorio");
        }

        [TestMethod]
        public async Task Modificar_ValidaElIdAntesQueElNombre()
        {
            var categoria = new Categoria { IdCategoria = 0, Nombre = string.Empty };

            await AfirmarRegla.ErrorAsync(() => _bl.ModificarAsync(categoria), "identificador de la categoría");
        }

        // ---------- EliminarAsync (desactivar) ----------

        [TestMethod]
        public async Task Eliminar_CategoriaNula_Rechaza()
        {
            await AfirmarRegla.ErrorAsync(() => _bl.EliminarAsync(null!), "no puede ser nula");
        }

        [TestMethod]
        [DataRow(0)]
        [DataRow(-1)]
        public async Task Eliminar_IdInvalido_Rechaza(int pId)
        {
            await AfirmarRegla.ErrorAsync(
                () => _bl.EliminarAsync(new Categoria { IdCategoria = pId }),
                "identificador de la categoría");
        }

        // ---------- ReactivarAsync ----------

        [TestMethod]
        [DataRow(0)]
        [DataRow(-1)]
        public async Task Reactivar_IdInvalido_Rechaza(int pId)
        {
            await AfirmarRegla.ErrorAsync(() => _bl.ReactivarAsync(pId), "identificador de la categoría");
        }

        // ---------- ObtenerPorIdAsync ----------

        [TestMethod]
        public async Task ObtenerPorId_CategoriaNula_Rechaza()
        {
            await AfirmarRegla.ErrorAsync(() => _bl.ObtenerPorIdAsync(null!), "no puede ser nula");
        }

        [TestMethod]
        [DataRow(0)]
        [DataRow(-1)]
        public async Task ObtenerPorId_IdInvalido_Rechaza(int pId)
        {
            await AfirmarRegla.ErrorAsync(
                () => _bl.ObtenerPorIdAsync(new Categoria { IdCategoria = pId }),
                "identificador de la categoría");
        }
    }
}
