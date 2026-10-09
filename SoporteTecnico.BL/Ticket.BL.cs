using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SoporteTecnico.DAL;
using SoporteTecnico.EN.Entidades;
using SoporteTecnico.EN.Enumeraciones;

namespace SoporteTecnico.BL
{
    public class TicketBL
    {
        public static async Task<Usuario?> ObtenerActorAsync(int idUsuario)
        {
            using (var dbContexto = new DbContexto())
            {
                return await dbContexto.Usuario
                    .Include(u => u.Rol)
                    .FirstOrDefaultAsync(u => u.IdUsuario == idUsuario);
            }
        }

        public static async Task<Ticket?> ObtenerExistenteAsync(int idTicket)
        {
            using (var dbContexto = new DbContexto())
            {
                return await dbContexto.Ticket.FindAsync(idTicket);
            }
        }

        public static bool ValidarAcceso(Ticket ticket, int idUsuario, int idRol)
        {
            if (idRol == 1) return true; // Administrador
            if (ticket.IdSolicitante == idUsuario || ticket.IdTecnico == idUsuario) return true;
            return false;
        }
    }
}