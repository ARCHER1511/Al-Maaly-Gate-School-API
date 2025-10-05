using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;

namespace Application.DependencyInjection
{
    public static class ApplicationDependencyInjection
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
        {
            // Here you can add application-level services, for example:
            // services.AddScoped<IYourService, YourServiceImplementation>();
            return services;
        }
    }
}
