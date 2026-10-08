using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SoporteTecnico.BL.Excepciones;
using SoporteTecnico.DAL;
using SoporteTecnico.EN.Entidades;
using SoporteTecnico.EN.Enumeraciones;

namespace SoporteTecnico.BL
{
    public class TicketBL
    {
        private const int TituloMaxLength = 150;
        private const int DescripcionMaxLength = 2000;
        public async Task<int> GuardarAsync(Ticket pTicket)
        {
            Validar(pTicket);
            Normalizar(pTicket);

            var categoria = await CategoriaDAL.ObtenerPorIdAsync(new Categoria { IdCategoria = pTicket.IdCategoria });
            if (categoria == null)
                throw new ReglaNegocioException("La categoría seleccionada no existe.");

            if (!categoria.Activo)
                throw new ReglaNegocioException("La categoría seleccionada está inactiva.");

            pTicket.Estado = EstadoTicket.Pendiente;
            pTicket.FechaCreacion = DateTime.Now;
            pTicket.FechaCierre = null;
            pTicket.Solucion = null;
            pTicket.IdTecnico = null;

            return await TicketDAL.GuardarAsync(pTicket);
        }

        public async Task<List<Ticket>> ObtenerTodosAsync()
        {
            return await TicketDAL.ObtenerTodosAsync();
        }

        public async Task<Ticket?> ObtenerPorIdAsync(int pIdTicket)
        {
            if (pIdTicket <= 0)
                throw new ReglaNegocioException("El identificador del ticket no es válido.");

            var ticket = await TicketDAL.ObtenerPorIdAsync(pIdTicket);
            return ticket.IdTicket == 0 ? null : ticket;
        }

        public async Task<List<HistorialEstado>> ObtenerHistorialAsync(int pIdTicket)
        {
            if (pIdTicket <= 0)
                throw new ReglaNegocioException("El identificador del ticket no es válido.");

            return await HistorialEstadoDAL.ObtenerPorTicketAsync(new HistorialEstado { IdTicket = pIdTicket });
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