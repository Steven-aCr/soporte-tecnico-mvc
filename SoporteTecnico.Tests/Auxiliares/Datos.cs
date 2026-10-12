using SoporteTecnico.BL.Excepciones;
using SoporteTecnico.EN.Entidades;
using SoporteTecnico.EN.Enumeraciones;

namespace SoporteTecnico.Tests.Auxiliares;

/// <summary>
/// Constructores de entidades válidas. Cada test modifica solo el campo que quiere invalidar.
/// </summary>
internal static class Datos
{
    public static Usuario UsuarioValido() => new()
    {
        Nombre = "Ana Pérez",
        Correo = "ana@soporte.test",
        IdRol = (int)RolUsuario.Solicitante
    };

    public static Categoria CategoriaValida() => new()
    {
        IdCategoria = 1,
        Nombre = "Hardware",
        Descripcion = "Problemas con equipos físicos"
    };

    public static Ticket TicketValido() => new()
    {
        Titulo = "No enciende el equipo",
        Descripcion = "El equipo no responde al presionar el botón de encendido.",
        Prioridad = PrioridadTicket.Media,
        IdCategoria = 1,
        IdSolicitante = 1
    };

    public static Comentario ComentarioValido() => new()
    {
        Contenido = "Ya revisé el cable de poder.",
        IdTicket = 1,
        IdUsuario = 1
    };

    public static string Texto(int longitud) => new('x', longitud);
}

/// <summary>
/// Verifica que una acción lance ReglaNegocioException con un fragmento del mensaje esperado.
/// Si la validación no se dispara, la ejecución llegaría al DAL y fallaría con otra excepción
/// (DbContexto.ConnectionString es null en pruebas), por lo que el test falla sin tocar SQL Server.
/// </summary>
internal static class AfirmarRegla
{
    public static async Task ErrorAsync(Func<Task> pAccion, string pFragmentoMensaje)
    {
        var ex = await Assert.ThrowsExactlyAsync<ReglaNegocioException>(pAccion);
        StringAssert.Contains(ex.Message, pFragmentoMensaje);
    }
}