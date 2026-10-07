
using SoporteTecnico.EN.Enumeraciones;

namespace SoporteTecnico.EN.Entidades
{
    public class HistorialEstado
    {
        public int IdHistorialEstado { get; set; }
        public EstadoTicket? EstadoAnterior { get; set; }
        public EstadoTicket EstadoNuevo { get; set; }
        public DateTime FechaCambio { get; set; } = DateTime.Now;
        public int IdTicket { get; set; }
        public int IdUsuario { get; set; }

        public Ticket Ticket { get; set; } = null!;
        public Usuario Usuario { get; set; } = null!;
    }
}