using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SoporteTecnico.EN.Entidades
{
    [Table("HistorialEstado")]
    public class HistorialEstado
    {
        [Key]
        [Column("IdHistorialEstado")]
        public int IdHistorial { get; set; }

        public int? EstadoAnterior { get; set; }

        public int EstadoNuevo { get; set; }

        public DateTime FechaCambio { get; set; } = DateTime.Now;

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