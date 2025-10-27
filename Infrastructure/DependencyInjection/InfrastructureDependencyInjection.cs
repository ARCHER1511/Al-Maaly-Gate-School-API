using Domain.Entities;
using Infrastructure.Data;
using Infrastructure.Interfaces;
using Infrastructure.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.DependencyInjection
{
    public static class InfrastructureDependencyInjection
    {
        public static IServiceCollection AddInfrastructureServices(
            this IServiceCollection services,
            IConfiguration configuration
        )
        {
            // Repositories DbContextFactory
            services.AddScoped<IDbContextFactory<AlMaalyGateSchoolContext>, DbContextFactory>();
            services.AddScoped<IAdminRepository, AdminRepository>();
            services.AddScoped<ITextAnswersRepository, TextAnswersRepository>();
            services.AddScoped<IAppRoleRepository, AppRoleRepository>();
            services.AddScoped<IAppUserRepository, AppUserRepository>();
            services.AddScoped<IAppUserRoleRepository, AppUserRoleRepository>();
            services.AddScoped<IClassAppointmentRepository, ClassAppointmentRepository>();
            services.AddScoped<IClassAssetsRepository, ClassAssetsRepository>();
            services.AddScoped<IClassRepository, ClassRepository>();
            services.AddScoped<IExamRepository, ExamRepository>();
            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            services.AddScoped<IParentRepository, ParentRepository>();
            services.AddScoped<IParentStudentRepository, ParentStudentRepository>();
            services.AddScoped<IQuestionRepository, QuestionRepository>();
            services.AddScoped<IStudentRepository, StudentRepository>();
            services.AddScoped<ISubjectRepository, SubjectRepository>();
            services.AddScoped<ITeacherRepository, TeacherRepository>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<INotificationRepository, NotificationRepository>();
            services.AddScoped<IUserNotificationRepository, UserNotificationRepository>();
            services.AddScoped<IFileRecordRepository, FileRecordRepository>();
            services.AddScoped<IChoicesRepository, ChoicesRepository>();
            services.AddScoped<ITrueAndFalsesRepository, TrueAndFalsesRepository>();

            return services;
        }

        public static IServiceCollection AddDatabase(
            this IServiceCollection services,
            IConfiguration config
        )
        {
            // DbContext
            services.AddDbContext<AlMaalyGateSchoolContext>(options =>
                options.UseSqlServer(config.GetConnectionString("DefaultConnection"))
            );
            return services;
        }

        public static IServiceCollection AddIdentitySetup(
            this IServiceCollection services,
            IConfiguration config
        )
        {
            // Identity setup
            services
                .AddIdentity<AppUser, AppRole>()
                .AddEntityFrameworkStores<AlMaalyGateSchoolContext>()
                .AddDefaultTokenProviders();
            return services;
        }
    }
}
