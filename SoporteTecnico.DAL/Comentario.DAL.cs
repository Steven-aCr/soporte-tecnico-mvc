using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SoporteTecnico.EN.Entidades;

namespace SoporteTecnico.DAL
{
    public class ComentarioDAL
    {
        public static async Task<int> GuardarAsync(Comentario pComentario)
        {
            int result = 0;
            try
            {
                using (var dbContexto = new DbContexto())
                {
                    dbContexto.Add(pComentario);
                    result = await dbContexto.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                result = 0;
                throw new Exception(ex.Message);
            }
            return result;
        }

        public static async Task<List<Comentario>> ObtenerPorTicketAsync(Comentario pComentario)
        {
            var result = new List<Comentario>();
            try
            {
                using (var dbContexto = new DbContexto())
                {
                    result = await dbContexto.Comentario
                        .Include(c => c.Usuario)
                        .Where(c => c.IdTicket == pComentario.IdTicket)
                        .OrderBy(c => c.FechaCreacion)
                        .ThenBy(c => c.IdComentario)
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