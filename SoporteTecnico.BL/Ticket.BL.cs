using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SoporteTecnico.BL.Excepciones;
using SoporteTecnico.DAL;
using SoporteTecnico.EN;
using SoporteTecnico.EN.Entidades;
using SoporteTecnico.EN.Enumeraciones;

namespace SoporteTecnico.BL
{
    public class TicketBL
    {
        private const int TituloMaxLength = LongitudesCampo.TicketTitulo;
        private const int DescripcionMaxLength = LongitudesCampo.TicketDescripcion;
        private const int SolucionMaxLength = LongitudesCampo.TicketSolucion;

        public async Task<int> GuardarAsync(Ticket pTicket)
        {
            Validar(pTicket);
            Normalizar(pTicket);

            var solicitante = await UsuarioDAL.ObtenerPorIdAsync(pTicket.IdSolicitante);
            if (solicitante == null || !solicitante.Activo)
                throw new ReglaNegocioException("El solicitante no existe o está inactivo.");

            var categoria = await CategoriaDAL.ObtenerPorIdAsync(new Categoria { IdCategoria = pTicket.IdCategoria });
            if (categoria == null)
                throw new ReglaNegocioException("La categoría seleccionada no existe.");

            if (!categoria.Activo)
                throw new ReglaNegocioException("La categoría seleccionada está inactiva.");

            var ahora = DateTime.UtcNow;
            pTicket.Estado = EstadoTicket.Pendiente;
            pTicket.FechaCreacion = ahora;
            pTicket.FechaCierre = null;
            pTicket.Solucion = null;
            pTicket.IdTecnico = null;

            pTicket.Historial.Add(new HistorialEstado
            {
                EstadoAnterior = null,
                EstadoNuevo = (int)EstadoTicket.Pendiente,
                FechaCambio = ahora,
                IdUsuario = pTicket.IdSolicitante
            });

            return await TicketDAL.GuardarAsync(pTicket);
        }

        public async Task<List<Ticket>> ListarAsync(int pIdActor, EstadoTicket? pEstado = null)
        {
            var actor = await ObtenerActorAsync(pIdActor);

            switch ((RolUsuario)actor.IdRol)
            {
                case RolUsuario.Administrador:
                    return await TicketDAL.ObtenerTodosAsync(pEstado: pEstado);
                case RolUsuario.Tecnico:
                    return await TicketDAL.ObtenerTodosAsync(pIdTecnico: actor.IdUsuario, pEstado: pEstado);
                case RolUsuario.Solicitante:
                    return await TicketDAL.ObtenerTodosAsync(pIdSolicitante: actor.IdUsuario, pEstado: pEstado);
                default:
                    throw new ReglaNegocioException("El rol del usuario no es válido.");
            }
        }

        public async Task<Ticket> ObtenerDetalleAsync(int pIdActor, int pIdTicket)
        {
            var actor = await ObtenerActorAsync(pIdActor);
            var ticket = await ObtenerExistenteAsync(pIdTicket);
            ValidarAcceso(actor, ticket);
            return ticket;
        }

        public async Task<List<HistorialEstado>> ObtenerHistorialAsync(int pIdActor, int pIdTicket)
        {
            var actor = await ObtenerActorAsync(pIdActor);
            var ticket = await ObtenerExistenteAsync(pIdTicket);
            ValidarAcceso(actor, ticket);

            return await HistorialEstadoDAL.ObtenerPorTicketAsync(new HistorialEstado { IdTicket = pIdTicket });
        }

        public async Task<int> AsignarAsync(int pIdActor, int pIdTicket, int pIdTecnico)
        {
            var actor = await ObtenerActorAsync(pIdActor);
            if ((RolUsuario)actor.IdRol != RolUsuario.Administrador)
                throw new ReglaNegocioException("Solo un administrador puede asignar tickets.");

            var ticket = await ObtenerExistenteAsync(pIdTicket);
            if (ticket.Estado != EstadoTicket.Pendiente)
                throw new ReglaNegocioException("Solo se puede asignar un técnico a un ticket pendiente.");

            var tecnico = await UsuarioDAL.ObtenerPorIdAsync(pIdTecnico);
            if (tecnico == null || !tecnico.Activo || (RolUsuario)tecnico.IdRol != RolUsuario.Tecnico)
                throw new ReglaNegocioException("Debe seleccionar un técnico activo.");

            ticket.IdTecnico = tecnico.IdUsuario;

            return await TicketDAL.ModificarAsync(ticket);
        }

        public async Task<int> IniciarAsync(int pIdActor, int pIdTicket)
        {
            var actor = await ObtenerActorAsync(pIdActor);
            var ticket = await ObtenerExistenteAsync(pIdTicket);
            ValidarTecnicoAsignado(actor, ticket);

            if (ticket.Estado != EstadoTicket.Pendiente)
                throw new ReglaNegocioException("Solo se puede iniciar un ticket pendiente.");

            return await CambiarEstadoAsync(ticket, EstadoTicket.EnProceso, actor.IdUsuario);
        }

        public async Task<int> ResolverAsync(int pIdActor, int pIdTicket, string pSolucion)
        {
            var actor = await ObtenerActorAsync(pIdActor);
            var ticket = await ObtenerExistenteAsync(pIdTicket);
            ValidarTecnicoAsignado(actor, ticket);

            if (ticket.Estado != EstadoTicket.EnProceso)
                throw new ReglaNegocioException("Solo se puede resolver un ticket en proceso.");

            if (string.IsNullOrWhiteSpace(pSolucion))
                throw new ReglaNegocioException("Debe escribir la solución para resolver el ticket.");

            if (pSolucion.Trim().Length > SolucionMaxLength)
                throw new ReglaNegocioException($"La solución no puede superar los {SolucionMaxLength} caracteres.");

            ticket.Solucion = pSolucion.Trim();
            return await CambiarEstadoAsync(ticket, EstadoTicket.Resuelto, actor.IdUsuario);
        }

        public async Task<int> CerrarAsync(int pIdActor, int pIdTicket)
        {
            var actor = await ObtenerActorAsync(pIdActor);
            var ticket = await ObtenerExistenteAsync(pIdTicket);

            bool esAdmin = (RolUsuario)actor.IdRol == RolUsuario.Administrador;
            bool esPropietario = ticket.IdSolicitante == actor.IdUsuario;
            if (!esAdmin && !esPropietario)
                throw new ReglaNegocioException("Solo el administrador o el solicitante del ticket pueden cerrarlo.");

            if (ticket.Estado != EstadoTicket.Resuelto)
                throw new ReglaNegocioException("Solo se puede cerrar un ticket resuelto.");

            ticket.FechaCierre = DateTime.UtcNow;
            return await CambiarEstadoAsync(ticket, EstadoTicket.Cerrado, actor.IdUsuario);
        }

        // ---------- Auxiliares (también los usa ComentarioBL) ----------

        internal static async Task<Usuario> ObtenerActorAsync(int pIdActor)
        {
            if (pIdActor <= 0)
                throw new ReglaNegocioException("No se pudo identificar al usuario.");

            var actor = await UsuarioDAL.ObtenerPorIdAsync(pIdActor);
            if (actor == null || !actor.Activo)
                throw new ReglaNegocioException("El usuario no existe o está inactivo.");

            return actor;
        }

        internal static async Task<Ticket> ObtenerExistenteAsync(int pIdTicket)
        {
            if (pIdTicket <= 0)
                throw new ReglaNegocioException("El identificador del ticket no es válido.");

            var ticket = await TicketDAL.ObtenerPorIdAsync(pIdTicket);
            if (ticket.IdTicket == 0)
                throw new ReglaNegocioException("El ticket no existe.");

            return ticket;
        }

        internal static void ValidarAcceso(Usuario pActor, Ticket pTicket)
        {
            bool acceso = (RolUsuario)pActor.IdRol switch
            {
                RolUsuario.Administrador => true,
                RolUsuario.Tecnico => pTicket.IdTecnico == pActor.IdUsuario,
                RolUsuario.Solicitante => pTicket.IdSolicitante == pActor.IdUsuario,
                _ => false
            };

            if (!acceso)
                throw new ReglaNegocioException("No tiene acceso a este ticket.");
        }

        private static void ValidarTecnicoAsignado(Usuario pActor, Ticket pTicket)
        {
            if ((RolUsuario)pActor.IdRol != RolUsuario.Tecnico || pTicket.IdTecnico != pActor.IdUsuario)
                throw new ReglaNegocioException("Solo el técnico asignado puede realizar esta acción.");
        }

        private static async Task<int> CambiarEstadoAsync(Ticket pTicket, EstadoTicket pNuevoEstado, int pIdActor)
        {
            var historial = new HistorialEstado
            {
                EstadoAnterior = (int)pTicket.Estado,
                EstadoNuevo = (int)pNuevoEstado,
                FechaCambio = DateTime.UtcNow,
                IdUsuario = pIdActor
            };

            pTicket.Estado = pNuevoEstado;
            return await TicketDAL.CambiarEstadoAsync(pTicket, historial);
        }

        private static void Validar(Ticket pTicket)
        {
            if (pTicket == null)
                throw new ReglaNegocioException("El ticket no puede ser nulo.");

            if (string.IsNullOrWhiteSpace(pTicket.Titulo))
                throw new ReglaNegocioException("El título es obligatorio.");

            if (pTicket.Titulo.Trim().Length > TituloMaxLength)
                throw new ReglaNegocioException($"El título no puede superar los {TituloMaxLength} caracteres.");

            if (string.IsNullOrWhiteSpace(pTicket.Descripcion))
                throw new ReglaNegocioException("La descripción es obligatoria.");

            if (pTicket.Descripcion.Trim().Length > DescripcionMaxLength)
                throw new ReglaNegocioException($"La descripción no puede superar los {DescripcionMaxLength} caracteres.");

            if (!Enum.IsDefined(typeof(PrioridadTicket), pTicket.Prioridad))
                throw new ReglaNegocioException("La prioridad seleccionada no es válida.");

            if (pTicket.IdCategoria <= 0)
                throw new ReglaNegocioException("Debe seleccionar una categoría.");

            if (pTicket.IdSolicitante <= 0)
                throw new ReglaNegocioException("El ticket debe tener un solicitante.");
        }

        private static void Normalizar(Ticket pTicket)
        {
            pTicket.Titulo = pTicket.Titulo.Trim();
            pTicket.Descripcion = pTicket.Descripcion.Trim();
        }
    }
}