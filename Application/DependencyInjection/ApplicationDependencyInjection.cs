using Application.Interfaces;
using Application.Mappings;
using Application.Services;
using Application.SignalR;
using AutoMapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Application.DependencyInjection
{
    public static class ApplicationDependencyInjection
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddAutoMapper(typeof(MappingProfile));
            var mapperConfig = new MapperConfiguration(cfg => {
                cfg.AddProfile<MappingProfile>();
            });
            try
            {
                mapperConfig.AssertConfigurationIsValid();
            }
            catch (Exception ex)
            {
                throw new Exception("AutoMapper configuration is invalid", ex);
            }

            services.AddScoped<IAdminService, AdminService>();
            services.AddScoped<INotificationService, NotificationService>();
            services.AddScoped<IUserNotificationService, UserNotificationService>();

            services.AddSignalR();
            services.AddScoped<INotificationBroadcaster, SignalRNotificationBroadcaster>();
            return services;
        }
    }
}
