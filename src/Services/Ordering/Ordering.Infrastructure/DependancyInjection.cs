using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;

namespace Ordering.Infrastructure
{
    public static class DependancyInjection
    {
        public static IServiceCollection AddInfrastructureServices(
            this IServiceCollection services, 
            IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("Database");

            // Add services to the container.
            
            // Add application services here
            return services;
        }
    }
}
