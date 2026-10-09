using Microsoft.EntityFrameworkCore;
using SoporteTecnico.EN;
using SoporteTecnico.EN.Entidades;
using System;
using System.Collections.Generic;
using System.Text;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;

namespace SoporteTecnico.DAL
{
    public class DbContexto : DbContext
    {
        // Esta propiedad recibe la cadena de conexión desde Program.cs
        public static string? ConnectionString { get; set; }

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
                {
                    throw new InvalidOperationException("La cadena de conexión no ha sido inicializada.");
                }

                // Configuración explícita para MySQL utilizando Pomelo.EntityFrameworkCore.MySql
                optionsBuilder.UseMySql(
                    ConnectionString,
                    ServerVersion.AutoDetect(ConnectionString)
                );
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuraciones adicionales de mapeo si las necesitas
            modelBuilder.Entity<Rol>().ToTable("Rol");
            modelBuilder.Entity<Usuario>().ToTable("Usuario");
            modelBuilder.Entity<Categoria>().ToTable("Categoria");
            modelBuilder.Entity<Ticket>().ToTable("Ticket");
            modelBuilder.Entity<Comentario>().ToTable("Comentario");
            modelBuilder.Entity<HistorialEstado>().ToTable("HistorialEstado");
        }
    }
}