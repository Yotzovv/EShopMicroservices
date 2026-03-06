using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace BuildingBlocks.Messaging.MasstTransit
{
    public static class Extensions
    {
        public static IServiceCollection AddMasstTransit(this IServiceCollection services, Assembly? assembly = null)
        {
            return services;
        }
    }
}
