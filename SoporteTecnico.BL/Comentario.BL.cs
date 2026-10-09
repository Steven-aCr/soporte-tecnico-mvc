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
    public class ComentarioBL
    {
        private const int ContenidoMaxLength = LongitudesCampo.ComentarioContenido;

        public async Task<int> GuardarAsync(Comentario pComentario)
        {
            Validar(pComentario);
            Normalizar(pComentario);

            var actor = await TicketBL.ObtenerActorAsync(pComentario.IdUsuario);
            var ticket = await TicketBL.ObtenerExistenteAsync(pComentario.IdTicket);
            TicketBL.ValidarAcceso(actor, ticket);

            if (ticket.Estado == EstadoTicket.Cerrado)
                throw new ReglaNegocioException("No se pueden agregar comentarios a un ticket cerrado.");

            pComentario.FechaCreacion = DateTime.UtcNow;
            return await ComentarioDAL.GuardarAsync(pComentario);
        }

        public async Task<List<Comentario>> ObtenerPorTicketAsync(int pIdActor, int pIdTicket)
        {
            var actor = await TicketBL.ObtenerActorAsync(pIdActor);
            var ticket = await TicketBL.ObtenerExistenteAsync(pIdTicket);
            TicketBL.ValidarAcceso(actor, ticket);

            return await ComentarioDAL.ObtenerPorTicketAsync(new Comentario { IdTicket = pIdTicket });
        }

        private static void Validar(Comentario pComentario)
        {
            if (pComentario == null)
                throw new ReglaNegocioException("El comentario no puede ser nulo.");

            if (pComentario.IdTicket <= 0)
                throw new ReglaNegocioException("El comentario debe pertenecer a un ticket.");

            if (pComentario.IdUsuario <= 0)
                throw new ReglaNegocioException("El comentario debe tener un autor.");

            if (string.IsNullOrWhiteSpace(pComentario.Contenido))
                throw new ReglaNegocioException("El comentario no puede estar vacío.");

            if (pComentario.Contenido.Trim().Length > ContenidoMaxLength)
                throw new ReglaNegocioException($"El comentario no puede superar los {ContenidoMaxLength} caracteres.");
        }

        private static void Normalizar(Comentario pComentario)
        {
            pComentario.Contenido = pComentario.Contenido.Trim();
        }
    }
}