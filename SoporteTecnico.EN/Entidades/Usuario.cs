using System.Collections.Generic;

namespace SoporteTecnico.EN.Entidades
{
    public class Usuario
    {
        public int IdUsuario { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Correo { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public bool Activo { get; set; } = true;
        public int IdRol { get; set; }
        public Rol Rol { get; set; } = null!;

        public ICollection<Ticket> TicketsSolicitados { get; set; } = new List<Ticket>();
        public ICollection<Ticket> TicketsAsignados { get; set; } = new List<Ticket>();
    }
}