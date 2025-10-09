using Domain.Entities;
using Infrastructure.Data;
using Infrastructure.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Infrastructure.Interfaces;
namespace Infrastructure.DependencyInjection
{
    public static class InfrastructureDependencyInjection
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {

            // DbContext
            services.AddDbContext<AlMaalyGateSchoolContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

            // Identity setup
            services.AddIdentity<AppUser, AppRole>()
                .AddEntityFrameworkStores<AlMaalyGateSchoolContext>()
                .AddDefaultTokenProviders();
            // Repositories
            services.AddScoped<IDbContextFactory<AlMaalyGateSchoolContext>, DbContextFactory>();
            services.AddScoped<IAdminRepository,AdminRepository>();
            services.AddScoped<IAnswerRepository,AnswerRepository>();
            services.AddScoped<IAppRoleRepository,AppRoleRepository>();
            services.AddScoped<IAppUserRepository,AppUserRepository>();
            services.AddScoped<IAppUserRoleRepository, AppUserRoleRepository>();
            services.AddScoped<IClassAppointmentRepository, ClassAppointmentRepository>();
            services.AddScoped<IClassAssetsRepository,ClassAssetsRepository>();
            services.AddScoped<IClassRepository,ClassRepository>();
            services.AddScoped<IClassSubjectRepository,ClassSubjectRepository>();
            services.AddScoped<IExamRepository,ExamRepository>();
            services.AddScoped(typeof(IGenericRepository<>),typeof(GenericRepository<>));
            services.AddScoped<IParentRepository,ParentRepository>();
            services.AddScoped<IParentStudentRepository,ParentStudentRepository>();
            services.AddScoped<IQuestionExamTeacherRepository,QuestionExamTeacherRepository>();
            services.AddScoped<IQuestionRepository,QuestionRepository>();
            services.AddScoped<IStudentQuestionAnswerExamRepository,StudentQuestionAnswerExamRepository>();
            services.AddScoped<IStudentRepository,StudentRepository>();
            services.AddScoped<IStudentSubjectExamRepository,StudentSubjectExamRepository>();
            services.AddScoped<ISubjectRepository,SubjectRepository>();
            services.AddScoped<ITeacherRepository,TeacherRepository>();
            services.AddScoped<ITeacherSubjectExamRepository,TeacherSubjectExamRepository>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<INotificationRepository, NotificationRepository>();
            services.AddScoped<IUserNotificationRepository, UserNotificationRepository>();
            

            return services;
        }
    }
}
