using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using SoporteTecnico.EN.Entidades;
using SoporteTecnico.DAL;

namespace SoporteTecnico.DAL
{
    public class HistorialEstadoDAL
    {
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