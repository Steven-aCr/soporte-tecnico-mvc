using SoporteTecnico.BL;
using SoporteTecnico.EN;
using SoporteTecnico.Tests.Auxiliares;

namespace SoporteTecnico.Tests.Servicios
{
    /// <summary>
    /// Validaciones de ComentarioBL que se resuelven antes de llegar al DAL.
    /// Los permisos y la restricción de tickets cerrados requieren datos del DAL y quedan pendientes.
    /// </summary>
    [TestClass]
    public class ComentarioBLTests
    {
        private readonly ComentarioBL _bl = new();

        [TestMethod]
        public async Task Guardar_ComentarioNulo_Rechaza()
        {
            await AfirmarRegla.ErrorAsync(() => _bl.GuardarAsync(null!), "no puede ser nulo");
        }

        [TestMethod]
        [DataRow(0)]
        [DataRow(-1)]
        public async Task Guardar_SinTicket_Rechaza(int pIdTicket)
        {
            var comentario = Datos.ComentarioValido();
            comentario.IdTicket = pIdTicket;

            await AfirmarRegla.ErrorAsync(() => _bl.GuardarAsync(comentario), "pertenecer a un ticket");
        }

        [TestMethod]
        [DataRow(0)]
        [DataRow(-1)]
        public async Task Guardar_SinAutor_Rechaza(int pIdUsuario)
        {
            var comentario = Datos.ComentarioValido();
            comentario.IdUsuario = pIdUsuario;

            await AfirmarRegla.ErrorAsync(() => _bl.GuardarAsync(comentario), "debe tener un autor");
        }

        [TestMethod]
        [DataRow((string?)null)]
        [DataRow("")]
        [DataRow("   ")]
        public async Task Guardar_ContenidoVacio_Rechaza(string? pContenido)
        {
            var comentario = Datos.ComentarioValido();
            comentario.Contenido = pContenido!;

            await AfirmarRegla.ErrorAsync(() => _bl.GuardarAsync(comentario), "no puede estar vacío");
        }

        [TestMethod]
        public async Task Guardar_ContenidoExcedeLongitud_Rechaza()
        {
            var comentario = Datos.ComentarioValido();
            comentario.Contenido = Datos.Texto(LongitudesCampo.ComentarioContenido + 1);

            await AfirmarRegla.ErrorAsync(
                () => _bl.GuardarAsync(comentario),
                $"{LongitudesCampo.ComentarioContenido} caracteres");
        }

        [TestMethod]
        public async Task Guardar_ValidaElTicketAntesQueElAutorYElContenido()
        {
            var comentario = Datos.ComentarioValido();
            comentario.IdTicket = 0;
            comentario.IdUsuario = 0;
            comentario.Contenido = string.Empty;

            await AfirmarRegla.ErrorAsync(() => _bl.GuardarAsync(comentario), "pertenecer a un ticket");
        }

        [TestMethod]
        [DataRow(0)]
        [DataRow(-1)]
        public async Task ObtenerPorTicket_ActorInvalido_Rechaza(int pIdActor)
        {
            await AfirmarRegla.ErrorAsync(() => _bl.ObtenerPorTicketAsync(pIdActor, 1), "identificar al usuario");
        }
    }
}