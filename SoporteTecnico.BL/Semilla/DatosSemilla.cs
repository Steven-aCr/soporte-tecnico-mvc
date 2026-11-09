using Microsoft.EntityFrameworkCore;
using SoporteTecnico.BL.Seguridad;
using SoporteTecnico.DAL;
using SoporteTecnico.EN.Entidades;
using SoporteTecnico.EN.Enumeraciones;

namespace SoporteTecnico.BL.Semilla
{
    /// <summary>
    /// Inserta los datos iniciales (roles, usuarios, categorías, tickets, comentarios e historial)
    /// cuando la base de datos está vacía. Es seguro llamarlo en cada arranque: si ya hay datos, no hace nada.
    /// Solo para desarrollo.
    /// </summary>
    public static class DatosSemilla
    {
        /// <summary>Contraseña de todos los usuarios de prueba.</summary>
        public const string PasswordPrueba = "Soporte123*";

        public static async Task SembrarAsync()
        {
            // DbContexto.ConnectionString ya fue asignada por AddDataAccess en Program.cs.
            await using var db = new DbContexto();

            await AsegurarRolesAsync(db);

            if (await db.Usuario.AnyAsync() || await db.Categoria.AnyAsync() || await db.Ticket.AnyAsync())
                return;

            // DateTime(0) en SQL Server: se trunca a segundos para que lo guardado coincida con lo calculado.
            var ahora = DateTime.UtcNow;
            ahora = new DateTime(ahora.Ticks - (ahora.Ticks % TimeSpan.TicksPerSecond), DateTimeKind.Utc);

            // ---------- Usuarios ----------
            var admin = NuevoUsuario("Administrador del Sistema", "admin@soporte.test", RolUsuario.Administrador);
            var carlos = NuevoUsuario("Carlos Martínez", "carlos@soporte.test", RolUsuario.Tecnico);
            var ana = NuevoUsuario("Ana Rivas", "ana@soporte.test", RolUsuario.Tecnico);
            var maria = NuevoUsuario("María López", "maria@soporte.test", RolUsuario.Solicitante);
            var jose = NuevoUsuario("José Hernández", "jose@soporte.test", RolUsuario.Solicitante);

            // ---------- Categorías ----------
            var hardware = new Categoria { Nombre = "Hardware", Descripcion = "Fallas en equipos, monitores, teclados y demás componentes físicos." };
            var software = new Categoria { Nombre = "Software", Descripcion = "Instalación, errores y licencias de aplicaciones." };
            var red = new Categoria { Nombre = "Red y conectividad", Descripcion = "Problemas de internet, Wi-Fi, VPN o acceso a recursos de red." };
            var cuentas = new Categoria { Nombre = "Cuentas y accesos", Descripcion = "Restablecimiento de contraseñas, permisos y alta o baja de cuentas." };
            var perifericos = new Categoria { Nombre = "Impresoras y periféricos", Descripcion = "Impresoras, escáneres y otros dispositivos conectados." };

            // ---------- T1: Pendiente (sin técnico) ----------
            var t1 = new Ticket
            {
                Titulo = "La computadora no enciende",
                Descripcion = "Desde esta mañana el equipo de mi escritorio no enciende. El botón de encendido no responde y no se escucha ningún sonido.",
                Prioridad = PrioridadTicket.Alta,
                Estado = EstadoTicket.Pendiente,
                FechaCreacion = ahora.AddHours(-3),
                Categoria = hardware,
                Solicitante = maria
            };
            t1.Historial.Add(Historial(null, EstadoTicket.Pendiente, ahora.AddHours(-3), maria));
            t1.Comentarios.Add(Comentario("Ya revisé el cable de poder y el tomacorriente, y el problema continúa.", ahora.AddHours(-2), maria));

            // ---------- T2: EnProceso (técnico Carlos) ----------
            var t2 = new Ticket
            {
                Titulo = "Internet muy lento en la oficina",
                Descripcion = "La conexión se vuelve lenta después del mediodía y varias páginas no cargan.",
                Prioridad = PrioridadTicket.Media,
                Estado = EstadoTicket.EnProceso,
                FechaCreacion = ahora.AddDays(-2),
                Categoria = red,
                Solicitante = jose,
                Tecnico = carlos
            };
            t2.Historial.Add(Historial(null, EstadoTicket.Pendiente, ahora.AddDays(-2), jose));
            t2.Historial.Add(Historial(EstadoTicket.Pendiente, EstadoTicket.EnProceso, ahora.AddDays(-2).AddHours(2), carlos));
            t2.Comentarios.Add(Comentario("Estoy revisando el router y el ancho de banda por hora. Les confirmo en breve.", ahora.AddDays(-1), carlos));
            t2.Comentarios.Add(Comentario("Gracias, el problema se presenta sobre todo entre las 12:00 y las 14:00.", ahora.AddHours(-20), jose));

            // ---------- T3: Resuelto (técnico Ana, con solución) ----------
            var t3 = new Ticket
            {
                Titulo = "Instalar Microsoft Office",
                Descripcion = "Necesito Office instalado en mi equipo nuevo para trabajar con hojas de cálculo.",
                Prioridad = PrioridadTicket.Baja,
                Estado = EstadoTicket.Resuelto,
                FechaCreacion = ahora.AddDays(-4),
                Solucion = "Se instaló Office y se activó la licencia corporativa. Se verificó que abre correctamente.",
                Categoria = software,
                Solicitante = maria,
                Tecnico = ana
            };
            t3.Historial.Add(Historial(null, EstadoTicket.Pendiente, ahora.AddDays(-4), maria));
            t3.Historial.Add(Historial(EstadoTicket.Pendiente, EstadoTicket.EnProceso, ahora.AddDays(-4).AddHours(6), ana));
            t3.Historial.Add(Historial(EstadoTicket.EnProceso, EstadoTicket.Resuelto, ahora.AddDays(-4).AddHours(16), ana));
            t3.Comentarios.Add(Comentario("Office instalado y funcionando. Quedo atenta a su confirmación.", ahora.AddDays(-4).AddHours(16), ana));

            // ---------- T4: Cerrado (técnico Carlos, solución y fecha de cierre) ----------
            var t4 = new Ticket
            {
                Titulo = "No puedo iniciar sesión en el sistema contable",
                Descripcion = "Aparece el mensaje de contraseña incorrecta aunque estoy seguro de que es la correcta.",
                Prioridad = PrioridadTicket.Alta,
                Estado = EstadoTicket.Cerrado,
                FechaCreacion = ahora.AddDays(-7),
                FechaCierre = ahora.AddDays(-5),
                Solucion = "La cuenta estaba bloqueada por intentos fallidos. Se desbloqueó y se restableció la contraseña.",
                Categoria = cuentas,
                Solicitante = jose,
                Tecnico = carlos
            };
            t4.Historial.Add(Historial(null, EstadoTicket.Pendiente, ahora.AddDays(-7), jose));
            t4.Historial.Add(Historial(EstadoTicket.Pendiente, EstadoTicket.EnProceso, ahora.AddDays(-7).AddHours(4), carlos));
            t4.Historial.Add(Historial(EstadoTicket.EnProceso, EstadoTicket.Resuelto, ahora.AddDays(-7).AddHours(30), carlos));
            t4.Historial.Add(Historial(EstadoTicket.Resuelto, EstadoTicket.Cerrado, ahora.AddDays(-5), jose));
            t4.Comentarios.Add(Comentario("Confirmado, ya puedo ingresar al sistema. Gracias por la ayuda.", ahora.AddDays(-5).AddMinutes(-30), jose));

            // Un solo SaveChanges: todo se guarda junto o no se guarda nada.
            db.Usuario.AddRange(admin, carlos, ana, maria, jose);
            db.Categoria.AddRange(hardware, software, red, cuentas, perifericos);
            db.Ticket.AddRange(t1, t2, t3, t4);
            await db.SaveChangesAsync();
        }

        // ---------- Auxiliares ----------

        /// <summary>
        /// Crea los roles si la tabla está vacía y verifica que sus Id coincidan con la enumeración RolUsuario,
        /// de la que dependen toda la capa BL y la DAL.
        /// </summary>
        private static async Task AsegurarRolesAsync(DbContexto db)
        {
            if (!await db.Rol.AnyAsync())
            {
                // Uno por uno para que el IDENTITY asigne 1, 2 y 3 en este orden.
                foreach (var nombre in new[] { "Administrador", "Tecnico", "Solicitante" })
                {
                    db.Rol.Add(new Rol { Nombre = nombre });
                    await db.SaveChangesAsync();
                }
            }

            var roles = await db.Rol.AsNoTracking().ToDictionaryAsync(r => r.IdRol, r => r.Nombre);

            foreach (var rol in Enum.GetValues<RolUsuario>())
            {
                if (!roles.TryGetValue((int)rol, out var nombre) || nombre != rol.ToString())
                    throw new InvalidOperationException(
                        $"La tabla Rol no coincide con la enumeración RolUsuario: se esperaba IdRol {(int)rol} = '{rol}'.");
            }
        }

        private static Usuario NuevoUsuario(string nombre, string correo, RolUsuario rol) => new()
        {
            Nombre = nombre,
            Correo = correo,
            PasswordHash = PasswordHasher.Hashear(PasswordPrueba),
            Activo = true,
            IdRol = (int)rol
        };

        private static HistorialEstado Historial(EstadoTicket? anterior, EstadoTicket nuevo, DateTime fecha, Usuario usuario) => new()
        {
            EstadoAnterior = anterior.HasValue ? (int)anterior.Value : null,
            EstadoNuevo = (int)nuevo,
            FechaCambio = fecha,
            Usuario = usuario
        };

        private static Comentario Comentario(string contenido, DateTime fecha, Usuario usuario) => new()
        {
            Contenido = contenido,
            FechaCreacion = fecha,
            Usuario = usuario
        };
    }
}