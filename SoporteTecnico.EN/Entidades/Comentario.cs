using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SoporteTecnico.EN.Entidades
{
    [Table("Comentario")]
    public class Comentario
    {
        [Key]
        public int IdComentario { get; set; }

        public string Contenido { get; set; } = string.Empty;

        public DateTime FechaCreacion { get; set; } = DateTime.Now;

        [Column("IdTicket")]
        public int IdTicket { get; set; }

        [ForeignKey("IdTicket")]
        public Ticket Ticket { get; set; } = null!;

        [Column("IdUsuario")]
        public int IdUsuario { get; set; }

        [ForeignKey("IdUsuario")]
        public Usuario Usuario { get; set; } = null!;
    }
}