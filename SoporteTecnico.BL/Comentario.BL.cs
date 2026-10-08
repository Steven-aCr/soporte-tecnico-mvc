using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SoporteTecnico.BL.Excepciones;
using SoporteTecnico.DAL;
using SoporteTecnico.EN.Entidades;

namespace SoporteTecnico.BL
{
    public class ComentarioBL
    {
        private const int ContenidoMaxLength = 1000;
        public async Task<int> GuardarAsync(Comentario pComentario)
        {
            Validar(pComentario);
            Normalizar(pComentario);

            var ticket = await TicketDAL.ObtenerPorIdAsync(pComentario.IdTicket);
            if (ticket.IdTicket == 0)
                throw new ReglaNegocioException("El ticket al que intenta comentar no existe.");

            if (ticket.FechaCierre.HasValue)
                throw new ReglaNegocioException("No se pueden agregar comentarios a un ticket cerrado.");

            pComentario.FechaCreacion = DateTime.Now;
            return await ComentarioDAL.GuardarAsync(pComentario);
        }

        public async Task<List<Comentario>> ObtenerPorTicketAsync(int pIdTicket)
        {
            if (pIdTicket <= 0)
                throw new ReglaNegocioException("El identificador del ticket no es válido.");

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