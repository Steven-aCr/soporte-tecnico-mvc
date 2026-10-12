using SoporteTecnico.BL;
using SoporteTecnico.EN;
using SoporteTecnico.EN.Enumeraciones;
using SoporteTecnico.Tests.Auxiliares;

namespace SoporteTecnico.Tests.Servicios
{
    /// <summary>
    /// Validaciones de TicketBL que se resuelven antes de llegar al DAL:
    /// datos del ticket (GuardarAsync) e identificación del actor (resto de operaciones).
    /// Las reglas de permisos y transiciones de estado requieren datos del DAL y quedan
    /// pendientes hasta poder sustituirlo (ver notas de la revisión).
    /// </summary>
    [TestClass]
    public class TicketBLTests
    {
        private readonly TicketBL _bl = new();

        // ---------- GuardarAsync: validación de datos ----------

        [TestMethod]
        public async Task Guardar_TicketNulo_Rechaza()
        {
            await AfirmarRegla.ErrorAsync(() => _bl.GuardarAsync(null!), "no puede ser nulo");
        }

        [TestMethod]
        [DataRow((string?)null)]
        [DataRow("")]
        [DataRow("   ")]
        public async Task Guardar_TituloVacio_Rechaza(string? pTitulo)
        {
            var ticket = Datos.TicketValido();
            ticket.Titulo = pTitulo!;

            await AfirmarRegla.ErrorAsync(() => _bl.GuardarAsync(ticket), "título es obligatorio");
        }

        [TestMethod]
        public async Task Guardar_TituloExcedeLongitud_Rechaza()
        {
            var ticket = Datos.TicketValido();
            ticket.Titulo = Datos.Texto(LongitudesCampo.TicketTitulo + 1);

            await AfirmarRegla.ErrorAsync(
                () => _bl.GuardarAsync(ticket),
                $"{LongitudesCampo.TicketTitulo} caracteres");
        }

        [TestMethod]
        [DataRow((string?)null)]
        [DataRow("")]
        [DataRow("   ")]
        public async Task Guardar_DescripcionVacia_Rechaza(string? pDescripcion)
        {
            var ticket = Datos.TicketValido();
            ticket.Descripcion = pDescripcion!;

            await AfirmarRegla.ErrorAsync(() => _bl.GuardarAsync(ticket), "descripción es obligatoria");
        }

        [TestMethod]
        public async Task Guardar_DescripcionExcedeLongitud_Rechaza()
        {
            var ticket = Datos.TicketValido();
            ticket.Descripcion = Datos.Texto(LongitudesCampo.TicketDescripcion + 1);

            await AfirmarRegla.ErrorAsync(
                () => _bl.GuardarAsync(ticket),
                $"{LongitudesCampo.TicketDescripcion} caracteres");
        }

        [TestMethod]
        [DataRow(0)]
        [DataRow(4)]
        [DataRow(99)]
        [DataRow(-1)]
        public async Task Guardar_PrioridadFueraDelEnum_Rechaza(int pPrioridad)
        {
            var ticket = Datos.TicketValido();
            ticket.Prioridad = (PrioridadTicket)pPrioridad;

            await AfirmarRegla.ErrorAsync(() => _bl.GuardarAsync(ticket), "prioridad seleccionada no es válida");
        }

        [TestMethod]
        [DataRow(0)]
        [DataRow(-1)]
        public async Task Guardar_CategoriaNoSeleccionada_Rechaza(int pIdCategoria)
        {
            var ticket = Datos.TicketValido();
            ticket.IdCategoria = pIdCategoria;

            await AfirmarRegla.ErrorAsync(() => _bl.GuardarAsync(ticket), "seleccionar una categoría");
        }

        [TestMethod]
        [DataRow(0)]
        [DataRow(-1)]
        public async Task Guardar_SinSolicitante_Rechaza(int pIdSolicitante)
        {
            var ticket = Datos.TicketValido();
            ticket.IdSolicitante = pIdSolicitante;

            await AfirmarRegla.ErrorAsync(() => _bl.GuardarAsync(ticket), "debe tener un solicitante");
        }

        [TestMethod]
        public async Task Guardar_ValidaElTituloAntesQueLaDescripcion()
        {
            var ticket = Datos.TicketValido();
            ticket.Titulo = string.Empty;
            ticket.Descripcion = string.Empty;

            await AfirmarRegla.ErrorAsync(() => _bl.GuardarAsync(ticket), "título es obligatorio");
        }

        // ---------- Actor no identificado (id <= 0) en cada operación ----------

        [TestMethod]
        [DataRow(0)]
        [DataRow(-1)]
        public async Task Listar_ActorInvalido_Rechaza(int pIdActor)
        {
            await AfirmarRegla.ErrorAsync(() => _bl.ListarAsync(pIdActor), "identificar al usuario");
        }

        [TestMethod]
        [DataRow(0)]
        [DataRow(-1)]
        public async Task Listar_ConFiltroDeEstado_ActorInvalido_Rechaza(int pIdActor)
        {
            await AfirmarRegla.ErrorAsync(
                () => _bl.ListarAsync(pIdActor, EstadoTicket.Pendiente),
                "identificar al usuario");
        }

        [TestMethod]
        [DataRow(0)]
        [DataRow(-1)]
        public async Task ObtenerDetalle_ActorInvalido_Rechaza(int pIdActor)
        {
            await AfirmarRegla.ErrorAsync(() => _bl.ObtenerDetalleAsync(pIdActor, 1), "identificar al usuario");
        }

        [TestMethod]
        [DataRow(0)]
        [DataRow(-1)]
        public async Task ObtenerHistorial_ActorInvalido_Rechaza(int pIdActor)
        {
            await AfirmarRegla.ErrorAsync(() => _bl.ObtenerHistorialAsync(pIdActor, 1), "identificar al usuario");
        }

        [TestMethod]
        [DataRow(0)]
        [DataRow(-1)]
        public async Task Asignar_ActorInvalido_Rechaza(int pIdActor)
        {
            await AfirmarRegla.ErrorAsync(() => _bl.AsignarAsync(pIdActor, 1, 2), "identificar al usuario");
        }

        [TestMethod]
        [DataRow(0)]
        [DataRow(-1)]
        public async Task Iniciar_ActorInvalido_Rechaza(int pIdActor)
        {
            await AfirmarRegla.ErrorAsync(() => _bl.IniciarAsync(pIdActor, 1), "identificar al usuario");
        }

        [TestMethod]
        [DataRow(0)]
        [DataRow(-1)]
        public async Task Resolver_ActorInvalido_Rechaza(int pIdActor)
        {
            await AfirmarRegla.ErrorAsync(
                () => _bl.ResolverAsync(pIdActor, 1, "Se reemplazó la fuente de poder."),
                "identificar al usuario");
        }

        [TestMethod]
        [DataRow(0)]
        [DataRow(-1)]
        public async Task Cerrar_ActorInvalido_Rechaza(int pIdActor)
        {
            await AfirmarRegla.ErrorAsync(() => _bl.CerrarAsync(pIdActor, 1), "identificar al usuario");
        }
    }
}
