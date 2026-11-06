using SoporteTecnico.EN.Entidades;

namespace SoporteTecnico.Web.ViewModels.Home
{
    public class HomeIndexViewModel
    {
        public int Pendientes { get; set; }
        public int EnProceso { get; set; }
        public int Resueltos { get; set; }
        public int Cerrados { get; set; }
        public int Total => Pendientes + EnProceso + Resueltos + Cerrados;

        public List<Ticket> Recientes { get; set; } = new();
    }
}