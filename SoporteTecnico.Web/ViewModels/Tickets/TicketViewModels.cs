using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using SoporteTecnico.EN;
using SoporteTecnico.EN.Entidades;
using SoporteTecnico.EN.Enumeraciones;

namespace SoporteTecnico.Web.ViewModels.Tickets
{
    public class TicketIndexViewModel
    {
        public List<Ticket> Tickets { get; set; } = new();
        public EstadoTicket? Estado { get; set; }
    }

    public class TicketCreateViewModel
    {
        [Required(ErrorMessage = "El título es obligatorio.")]
        [StringLength(LongitudesCampo.TicketTitulo, ErrorMessage = "El título no puede superar {1} caracteres.")]
        [Display(Name = "Título")]
        public string Titulo { get; set; } = string.Empty;

        [Required(ErrorMessage = "La descripción es obligatoria.")]
        [StringLength(LongitudesCampo.TicketDescripcion, ErrorMessage = "La descripción no puede superar {1} caracteres.")]
        [Display(Name = "Descripción")]
        public string Descripcion { get; set; } = string.Empty;

        [Display(Name = "Prioridad")]
        public PrioridadTicket Prioridad { get; set; } = PrioridadTicket.Media;

        [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar una categoría.")]
        [Display(Name = "Categoría")]
        public int IdCategoria { get; set; }

        // Solo categorías activas
        public List<SelectListItem> Categorias { get; set; } = new();
    }

    public class TicketDetailsViewModel
    {
        public Ticket Ticket { get; set; } = null!;
        public List<Comentario> Comentarios { get; set; } = new();
        public List<HistorialEstado> Historial { get; set; } = new();

        // Técnicos activos; solo hace falta llenarlo si el actor es administrador
        public List<Usuario> Tecnicos { get; set; } = new();

        public int IdUsuarioActual { get; set; }
        public RolUsuario Rol { get; set; }

        public bool EsAdmin => Rol == RolUsuario.Administrador;
        public bool EsTecnicoAsignado => Rol == RolUsuario.Tecnico && Ticket.IdTecnico == IdUsuarioActual;

        public bool PuedeAsignar => EsAdmin && Ticket.Estado == EstadoTicket.Pendiente;
        public bool PuedeIniciar => EsTecnicoAsignado && Ticket.Estado == EstadoTicket.Pendiente;
        public bool PuedeResolver => EsTecnicoAsignado && Ticket.Estado == EstadoTicket.EnProceso;
        public bool PuedeCerrar => Ticket.Estado == EstadoTicket.Resuelto
                                   && (EsAdmin || Ticket.IdSolicitante == IdUsuarioActual);
        public bool PuedeComentar => Ticket.Estado != EstadoTicket.Cerrado;
    }
}