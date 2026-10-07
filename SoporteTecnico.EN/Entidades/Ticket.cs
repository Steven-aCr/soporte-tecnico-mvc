
using SoporteTecnico.EN.Enumeraciones;

namespace SoporteTecnico.EN.Entidades
{
    public class Ticket
    {
        public int IdTicket { get; set; }
        public string Titulo { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public PrioridadTicket Prioridad { get; set; } = PrioridadTicket.Media;
        public EstadoTicket Estado { get; set; } = EstadoTicket.Pendiente;
        public DateTime FechaCreacion { get; set; } = DateTime.Now;
        public DateTime? FechaCierre { get; set; }
        public string? Solucion { get; set; }
        public int IdCategoria { get; set; }
        public int IdSolicitante { get; set; }
        public int? IdTecnico { get; set; }

        public Categoria Categoria { get; set; } = null!;
        public Usuario Solicitante { get; set; } = null!;
        public Usuario? Tecnico { get; set; }

        public ICollection<Comentario> Comentarios { get; set; } = new List<Comentario>();
        public ICollection<HistorialEstado> Historial { get; set; } = new List<HistorialEstado>();
    
    }
}