using Microsoft.Extensions.DependencyInjection;

namespace SoporteTecnico.BL
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Registra los servicios de negocio. Se llama desde Program.cs de SoporteTecnico.Web.
        /// </summary>
        public static IServiceCollection AddSoporteTecnicoBL(this IServiceCollection services)
        {
            services.AddScoped<UsuarioBL>();
            services.AddScoped<CategoriaBL>();
            services.AddScoped<TicketBL>();
            services.AddScoped<ComentarioBL>();
            return services;
        }
    }
}