
namespace SoporteTecnico.EN.Entidades
{
    public class Comentario
    {
        public int IdComentario { get; set; }
        public string Contenido { get; set; } = string.Empty;
        public DateTime FechaCreacion { get; set; } = DateTime.Now;
        public int IdTicket { get; set; }
        public int IdUsuario { get; set; }


        public Ticket Ticket { get; set; } = null!;
        public Usuario Usuario { get; set; } = null!;
    }
}