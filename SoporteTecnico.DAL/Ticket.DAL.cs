using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SoporteTecnico.EN.Entidades;

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

        public static async Task<List<Ticket>> ObtenerTodosAsync()
        {
            try
            {
                using (var dbContexto = new DbContexto())
                {
                    return await dbContexto.Ticket
                        .Include(t => t.Categoria)
                        .Include(t => t.Solicitante)
                        .Include(t => t.Tecnico)
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
    }
}