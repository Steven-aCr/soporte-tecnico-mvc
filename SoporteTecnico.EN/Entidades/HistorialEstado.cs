
using SoporteTecnico.EN.Enumeraciones;
using System;

namespace SoporteTecnico.EN.Entidades
{
    public class HistorialEstado
    {
       public int IdHistorial { get; set; }
        public EstadoTicket? EstadoAnterior { get; set; }
        public EstadoTicket EstadoNuevo { get; set; }
        public DateTime FechaCambio { get; set; } = DateTime.Now;
        public int IdTicket { get; set; }
        public int IdUsuario { get; set; }

        public Ticket Ticket { get; set; } = null!;
        public Usuario Usuario { get; set; } = null!;
    }
}