using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SoporteTecnico.DAL.Interfaces;

namespace SoporteTecnico.DAL
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddDataAccess(this IServiceCollection services, string connectionString)
        {
            DbContexto.ConnectionString = connectionString;

            services.AddDbContext<DbContexto>(options =>
                options.UseSqlServer(connectionString));
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            return services;
        }
    }
}