using Application.DependencyInjection;
using Application.SignalR;
using Common.Extensions;
using Infrastructure.DependencyInjection;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.OpenApi.Models;
using QuestPDF.Infrastructure;
using System.Text.Json.Serialization;


namespace Al_Maaly_Gate_School
{
    public class Program
    {
        public void Main() { }
        public static void Main(string[] args)
        {
            QuestPDF.Settings.License = LicenseType.Community;

            var builder = WebApplication.CreateBuilder(args);
            var configuration = builder.Configuration;

            // Add services to the container.
            builder.Services.AddDatabase(builder.Configuration);
            builder.Services.AddIdentitySetup(builder.Configuration);
            builder.Services.AddInfrastructureServices(builder.Configuration);
            builder.Services.AddApplicationServices(builder.Configuration);
            builder.Services.AddCustomCORS(builder.Configuration);
            builder.Services.AddJwtAuthentication(builder.Configuration);
            builder
                .Services.AddControllers(opt =>
                {
                    var policy = new AuthorizationPolicyBuilder("Bearer").RequireAuthenticatedUser().Build();
                    opt.Filters.Add(new AuthorizeFilter(policy));
                })
                .AddJsonOptions(opt =>
                {
                    opt.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
                    opt.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                });
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddHttpsRedirection(options =>
            {
                options.HttpsPort = 7002;
            });
            builder.Services.AddAuthorization();

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new() { Title = "Al-Maaly-Gate-School API", Version = "v1" });
                c.OperationFilter<FileUploadOperationFilter>();
                c.AddSecurityDefinition(
                    "Bearer",
                    new OpenApiSecurityScheme
                    {
                        Name = "Authorization",
                        Type = SecuritySchemeType.Http,
                        Scheme = "bearer",
                        BearerFormat = "JWT",
                        In = ParameterLocation.Header,
                        Description = "Enter 'Bearer' followed by your token",
                    }
                );
                c.AddSecurityRequirement(
                    new OpenApiSecurityRequirement
                    {
                        {
                            new OpenApiSecurityScheme
                            {
                                Reference = new OpenApiReference
                                {
                                    Type = ReferenceType.SecurityScheme,
                                    Id = "Bearer",
                                },
                            },
                            Array.Empty<string>()
                        },
                    }
                );
            });

            var app = builder.Build();

            app.UseStaticFiles();
            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseRouting();

            app.UseCustomCORS();

            app.UseAuthentication();
            app.UseAuthorization();
            app.MapHub<NotificationHub>("/hubs/notifications");

            app.MapControllers();

            app.Run();
        }
    }
}
