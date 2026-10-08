using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SoporteTecnico.EN.Entidades;
using SoporteTecnico.EN.Enumeraciones;

namespace SoporteTecnico.DAL
{
    public class HistorialEstadoDAL
    {
        public static async Task<int> GuardarAsync(HistorialEstado pHistorial)
        {
            int result = 0;
            try
            {
                using (var dbContexto = new DbContexto())
                {
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

        public static async Task<List<HistorialEstado>> ObtenerPorTicketAsync(HistorialEstado pHistorial)
        {
            var result = new List<HistorialEstado>();
            try
            {
                using (var dbContexto = new DbContexto())
                {
                    result = await dbContexto.HistorialEstado
                        .Include(h => h.Usuario)
                        .Where(h => h.IdTicket == pHistorial.IdTicket)
                        .OrderBy(h => h.FechaCambio)
                        .ThenBy(h => h.IdHistorial)
                        .ToListAsync();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return result;
        }
    }
}