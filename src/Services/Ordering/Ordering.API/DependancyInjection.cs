namespace Ordering.API
{
    public static class DependancyInjection
    {
        public static IServiceCollection AddApiServices(this IServiceCollection services)
        {
            // Add Carter.
            
            // Add application services here
            return services;
        }

        public static WebApplication UseApiServices(this WebApplication app)
        {
            // Configure the HTTP request pipeline.
            return app;
        }
    }
}
