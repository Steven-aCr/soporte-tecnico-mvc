namespace SoporteTecnico.EN.Enumeraciones
{
    /// <summary>
    /// Roles del sistema. Los valores asumen que dbo.Rol tiene IdRol 1, 2 y 3
    /// en este orden (ver verificación al final).
    /// </summary>
    public enum RolUsuario
    {
        Administrador = 1,
        Tecnico = 2,
        Solicitante = 3
    }
}
