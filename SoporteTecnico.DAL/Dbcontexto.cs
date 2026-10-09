using Microsoft.EntityFrameworkCore;
using SoporteTecnico.EN;
using SoporteTecnico.EN.Entidades;

namespace SoporteTecnico.DAL
{
    public class DbContexto : DbContext
    {
        /// <summary>Se asigna una vez en Program.cs desde appsettings.json.</summary>
        public static string? ConnectionString { get; set; }

        // AutoDetect abre una conexión; se hace una sola vez y se reutiliza en cada DbContexto.
        private static ServerVersion? _serverVersion;

        public DbSet<Rol> Rol { get; set; }
        public DbSet<Usuario> Usuario { get; set; }
        public DbSet<Categoria> Categoria { get; set; }
        public DbSet<Ticket> Ticket { get; set; }
        public DbSet<Comentario> Comentario { get; set; }
        public DbSet<HistorialEstado> HistorialEstado { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                if (string.IsNullOrEmpty(ConnectionString))
                    throw new InvalidOperationException("La cadena de conexión no ha sido inicializada.");

                _serverVersion ??= ServerVersion.AutoDetect(ConnectionString);
                optionsBuilder.UseMySql(ConnectionString, _serverVersion);
            }
        }

        protected override void OnModelCreating(ModelBuilder mb)
        {
            base.OnModelCreating(mb);

            mb.Entity<Rol>(e =>
            {
                e.ToTable("Rol");
                e.HasKey(x => x.IdRol);
                e.Property(x => x.IdRol).ValueGeneratedNever(); // IDs fijos 1, 2, 3
                e.Property(x => x.Nombre).IsRequired().HasMaxLength(LongitudesCampo.RolNombre);
                e.HasIndex(x => x.Nombre).IsUnique();
            });

            mb.Entity<Usuario>(e =>
            {
                e.ToTable("Usuario");
                e.HasKey(x => x.IdUsuario);
                e.Property(x => x.Nombre).IsRequired().HasMaxLength(LongitudesCampo.UsuarioNombre);
                e.Property(x => x.Correo).IsRequired().HasMaxLength(LongitudesCampo.UsuarioCorreo);
                e.Property(x => x.PasswordHash).IsRequired().HasMaxLength(LongitudesCampo.UsuarioPasswordHash);
                e.HasIndex(x => x.Correo).IsUnique();

                e.HasOne(x => x.Rol)
                 .WithMany(r => r.Usuarios)
                 .HasForeignKey(x => x.IdRol)
                 .OnDelete(DeleteBehavior.Restrict);
            });

            mb.Entity<Categoria>(e =>
            {
                e.ToTable("Categoria");
                e.HasKey(x => x.IdCategoria);
                e.Property(x => x.Nombre).IsRequired().HasMaxLength(LongitudesCampo.CategoriaNombre);
                e.Property(x => x.Descripcion).HasMaxLength(LongitudesCampo.CategoriaDescripcion);
                e.HasIndex(x => x.Nombre).IsUnique();
            });

            mb.Entity<Ticket>(e =>
            {
                e.ToTable("Ticket");
                e.HasKey(x => x.IdTicket);
                e.Property(x => x.Titulo).IsRequired().HasMaxLength(LongitudesCampo.TicketTitulo);
                e.Property(x => x.Descripcion).IsRequired().HasMaxLength(LongitudesCampo.TicketDescripcion);
                e.Property(x => x.Solucion).HasMaxLength(LongitudesCampo.TicketSolucion);
                e.Property(x => x.Prioridad).HasConversion<int>();
                e.Property(x => x.Estado).HasConversion<int>();

                e.HasOne(x => x.Categoria)
                 .WithMany(c => c.Tickets)
                 .HasForeignKey(x => x.IdCategoria)
                 .OnDelete(DeleteBehavior.Restrict);

                // Dos relaciones distintas hacia Usuario: hay que configurarlas por separado.
                e.HasOne(x => x.Solicitante)
                 .WithMany(u => u.TicketsSolicitados)
                 .HasForeignKey(x => x.IdSolicitante)
                 .OnDelete(DeleteBehavior.Restrict);

                e.HasOne(x => x.Tecnico)
                 .WithMany(u => u.TicketsAsignados)
                 .HasForeignKey(x => x.IdTecnico)
                 .OnDelete(DeleteBehavior.Restrict);
            });

            mb.Entity<Comentario>(e =>
            {
                e.ToTable("Comentario");
                e.HasKey(x => x.IdComentario);
                e.Property(x => x.Contenido).IsRequired().HasMaxLength(LongitudesCampo.ComentarioContenido);

                e.HasOne(x => x.Ticket)
                 .WithMany(t => t.Comentarios)
                 .HasForeignKey(x => x.IdTicket)
                 .OnDelete(DeleteBehavior.Restrict);

                e.HasOne(x => x.Usuario)
                 .WithMany()
                 .HasForeignKey(x => x.IdUsuario)
                 .OnDelete(DeleteBehavior.Restrict);
            });

            mb.Entity<HistorialEstado>(e =>
            {
                e.ToTable("HistorialEstado");
                e.HasKey(x => x.IdHistorial);
                // Si en TU tabla de MySQL la columna se llama IdHistorialEstado, quita las barras de la línea siguiente:
                // e.Property(x => x.IdHistorial).HasColumnName("IdHistorialEstado");
                e.Property(x => x.EstadoAnterior).HasConversion<int?>();
                e.Property(x => x.EstadoNuevo).HasConversion<int>();

                e.HasOne(x => x.Ticket)
                 .WithMany(t => t.Historial)
                 .HasForeignKey(x => x.IdTicket)
                 .OnDelete(DeleteBehavior.Restrict);

                e.HasOne(x => x.Usuario)
                 .WithMany()
                 .HasForeignKey(x => x.IdUsuario)
                 .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}