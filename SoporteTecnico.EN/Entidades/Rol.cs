using SoporteTecnico.EN.Enumeraciones;

namespace SoporteTecnico.EN.Entidades
{
    public class Rol
    {
        public int IdRol { get; set; }
        public string Nombre { get; set; } = string.Empty;

        public ICollection<RolUsuario> Usuarios { get; set; } = new List<Usuario>();
    }
}
