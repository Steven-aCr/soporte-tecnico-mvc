using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SoporteTecnico.EN.Entidades
{
    [Table("Usuario")]
    public class Usuario
    {
        [Key]
        public int IdUsuario { get; set; }

        public string Nombre { get; set; } = string.Empty;

        public string Correo { get; set; } = string.Empty;

        public string PasswordHash { get; set; } = string.Empty;

        public bool Activo { get; set; } = true;

        [Column("IdRol")]
        public int IdRol { get; set; }

        [ForeignKey("IdRol")]
        public Rol Rol { get; set; } = null!;

        public ICollection<Ticket> TicketsSolicitados { get; set; } = new List<Ticket>();
        public ICollection<Ticket> TicketsAsignados { get; set; } = new List<Ticket>();
    }
}