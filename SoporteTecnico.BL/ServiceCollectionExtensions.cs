using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
namespace SoporteTecnico.BL
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddSoporteTecnicoBL(this IServiceCollection services)
        {
            services.AddScoped<CategoriaBL>();
            return services;
        }
    }
}