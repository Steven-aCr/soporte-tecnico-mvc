using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SoporteTecnico.EN.Entidades;
using SoporteTecnico.EN.Enumeraciones;

namespace SoporteTecnico.DAL
{
    public class TicketDAL
    {
        public static async Task<int> GuardarAsync(Ticket pTicket)
        {
            int result = 0;
            try
            {
                using (var dbContexto = new DbContexto())
                {
                    dbContexto.Add(pTicket);
                    result = await dbContexto.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return result;
        }

        public static async Task<int> ModificarAsync(Ticket pTicket)
        {
            int result = 0;
            try
            {
                using (var dbContexto = new DbContexto())
                {
                    var ticket = await dbContexto.Ticket.FirstOrDefaultAsync(
                        t => t.IdTicket == pTicket.IdTicket);

                    if (ticket == null)
                        throw new Exception($"No se encontró el ticket con ID {pTicket.IdTicket}.");

                    AplicarCambios(ticket, pTicket);

                    dbContexto.Update(ticket);
                    result = await dbContexto.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return result;
        }

        public static async Task<int> CambiarEstadoAsync(Ticket pTicket, HistorialEstado pHistorial)
        {
            int result = 0;
            try
            {
                using (var dbContexto = new DbContexto())
                {
                    var ticket = await dbContexto.Ticket.FirstOrDefaultAsync(
                        t => t.IdTicket == pTicket.IdTicket);

                    if (ticket == null)
                        throw new Exception($"No se encontró el ticket con ID {pTicket.IdTicket}.");

                    AplicarCambios(ticket, pTicket);

                    pHistorial.IdTicket = ticket.IdTicket;
                    dbContexto.Update(ticket);
                    dbContexto.Add(pHistorial);

                    result = await dbContexto.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return result;
        }

        public static async Task<List<Ticket>> ObtenerTodosAsync(
            int? pIdSolicitante = null,
            int? pIdTecnico = null,
            EstadoTicket? pEstado = null)
        {
            try
            {
                using (var dbContexto = new DbContexto())
                {
                    return await dbContexto.Ticket
                        .Include(t => t.Categoria)
                        .Include(t => t.Solicitante)
                        .Include(t => t.Tecnico)
                        .Where(t =>
                            (pIdSolicitante == null || t.IdSolicitante == pIdSolicitante) &&
                            (pIdTecnico == null || t.IdTecnico == pIdTecnico) &&
                            (pEstado == null || t.Estado == pEstado))
                        .OrderByDescending(t => t.FechaCreacion)
                        .ToListAsync();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public static async Task<Ticket> ObtenerPorIdAsync(int id)
        {
            try
            {
                using (var dbContexto = new DbContexto())
                {
                    var ticket = await dbContexto.Ticket
                        .Include(t => t.Categoria)
                        .Include(t => t.Solicitante)
                        .Include(t => t.Tecnico)
                        .FirstOrDefaultAsync(t => t.IdTicket == id);

                    return ticket ?? new Ticket();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private static void AplicarCambios(Ticket pDestino, Ticket pOrigen)
        {
            pDestino.Estado = pOrigen.Estado;
            pDestino.IdTecnico = pOrigen.IdTecnico;
            pDestino.Solucion = pOrigen.Solucion;
            pDestino.FechaCierre = pOrigen.FechaCierre;
        }
    }
}