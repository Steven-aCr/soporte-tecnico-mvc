using Microsoft.EntityFrameworkCore;
using SoporteTecnico.EN;
using SoporteTecnico.EN.Entidades;
using System;
using System.Collections.Generic;
using System.Text;

namespace SoporteTecnico.DAL
{
    /// <summary>
    /// Contexto de EF Core que mapea las entidades al esquema SQL existente.
    /// Se instancia con <c>new DbContexto()</c> dentro de cada método de las clases DAL.
    /// </summary>
    public class DbContexto : DbContext
    {
        /// <summary>
        /// Cadena de conexión. Se asigna una vez en Program.cs:
        /// <c>DbContexto.ConnectionString = builder.Configuration.GetConnectionString("SoporteTecnicoDB")!;</c>
        /// </summary>
        public static string ConnectionString { get; set; } = string.Empty;

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
                if (string.IsNullOrWhiteSpace(ConnectionString))
                    throw new InvalidOperationException(
                        "DbContexto.ConnectionString no está configurada. Asígnala en Program.cs.");

                optionsBuilder.UseSqlServer(ConnectionString);
            }
        }

        protected override void OnModelCreating(ModelBuilder mb)
        {
            mb.HasDefaultSchema("dbo");

            mb.Entity<Rol>(e =>
            {
                e.ToTable("Rol");
                e.HasKey(x => x.IdRol);
                e.Property(x => x.IdRol).ValueGeneratedNever(); // IDs fijos 1,2,3 sembrados por SQL
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
                e.Property(x => x.FechaCreacion).HasColumnType("datetime2(0)");
                e.Property(x => x.FechaCierre).HasColumnType("datetime2(0)");

                e.HasOne(x => x.Categoria)
                 .WithMany(c => c.Tickets)
                 .HasForeignKey(x => x.IdCategoria)
                 .OnDelete(DeleteBehavior.Restrict);

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
                e.Property(x => x.FechaCreacion).HasColumnType("datetime2(0)");

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
                e.Property(x => x.IdHistorial).HasColumnName("IdHistorialEstado"); // en el SQL la PK se llama IdHistorialEstado
                e.Property(x => x.EstadoAnterior).HasConversion<int?>();
                e.Property(x => x.EstadoNuevo).HasConversion<int>();
                e.Property(x => x.FechaCambio).HasColumnType("datetime2(0)");

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