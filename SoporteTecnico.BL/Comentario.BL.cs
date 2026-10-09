using Microsoft.EntityFrameworkCore;
using SoporteTecnico.BL.Excepciones;
using SoporteTecnico.DAL;
using SoporteTecnico.EN;
using SoporteTecnico.EN.Entidades;
using SoporteTecnico.EN.Enumeraciones;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SoporteTecnico.BL
{
    public class ComentarioBL
    {
        private const int ContenidoMaxLength = LongitudesCampo.ComentarioContenido;

        // Método auxiliar para obtener el usuario/actor actual (por ejemplo, por ID)
        public static async Task<Usuario?> ObtenerActorAsync(int idUsuario)
        {
            using (var dbContexto = new DbContexto())
            {
                return await dbContexto.Usuario
                    .Include(u => u.Rol)
                    .FirstOrDefaultAsync(u => u.IdUsuario == idUsuario);
            }
        }

        // Método auxiliar para verificar si un ticket existe antes de modificarlo/consultarlo
        public static async Task<Ticket?> ObtenerExistenteAsync(int idTicket)
        {
            using (var dbContexto = new DbContexto())
            {
                return await dbContexto.Ticket.FindAsync(idTicket);
            }
        }

        // Método auxiliar para validar permisos o acceso al ticket
        public static bool ValidarAcceso(Ticket ticket, int idUsuario, int idRol)
        {
            // Si el rol es administrador (ej. Rol 1) o es el solicitante/técnico asignado, tiene acceso
            if (idRol == 1) return true;
            if (ticket.IdSolicitante == idUsuario || ticket.IdTecnico == idUsuario) return true;

            return false;
        }
    }
}