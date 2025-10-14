using Application.Authentication;
using Application.Interfaces;
using Application.Mappings;
using Application.Services;
using Application.SignalR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Application.DependencyInjection
{
    public static class ApplicationDependencyInjection
    {
        public static IServiceCollection AddApplicationServices(
            this IServiceCollection services,
            IConfiguration configuration
        )
        {
            services.AddAutoMapper(typeof(MappingProfile));
            
            services.AddScoped<IAdminService, AdminService>();
            services.AddScoped<IAuthenticationService, AuthenticationService>();
            
            services.AddScoped<INotificationService, NotificationService>();
            services.AddScoped<IUserNotificationService, UserNotificationService>();
            //SignalR
            services.AddSignalR();
            services.AddScoped<INotificationBroadcaster, SignalRNotificationBroadcaster>();
            return services;
        }
    }
}
