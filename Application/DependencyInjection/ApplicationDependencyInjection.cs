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
            services.AddScoped<IAdminManagementService, AdminManagementService>();
            services.AddScoped<IAuthenticationService, AuthenticationService>();
            services.AddScoped<ITeacherService, TeacherService>();
            services.AddScoped<INotificationService, NotificationService>();
            services.AddScoped<IUserNotificationService, UserNotificationService>();
            services.AddScoped<IFileService, FileService>();
            services.AddScoped<IQuestionService, QuestionService>();
            services.AddScoped<IStudentService, StudentService>();
            services.AddScoped<IClassService, ClassService>();
            services.AddScoped<IExamService, ExamService>();
            services.AddScoped<ISubjectService, SubjectService>();
            services.AddScoped<IClassAppointmentService, ClassAppointmentService>();
            services.AddScoped<IStudentExamAnswerService, StudentExamAnswerService>();
            services.AddScoped<IStudentExamResultService, StudentExamResultService>();
            services.AddScoped<IManagementStudentService, ManagementStudentService>();
            services.AddScoped<IManagementTeacherService, ManagementTeacherService>();
            services.AddScoped<IManagementClassService, ManagementClassService>();
            services.AddScoped<IManagementSubjectService, ManagementSubjectService>();
            services.AddScoped<IManagementParentService, ManagementParentService>();
            services.AddScoped<IDegreeService, DegreeService>();
            services.AddScoped<IGpaCalculator, GpaCalculator>();
            services.AddScoped<ICertificateService, CertificateService>();
            services.AddScoped<IGradeService, GradeService>();
<<<<<<< HEAD
            services.AddScoped<ICurriculumService, CurriculumService>();
            services.AddScoped<ICertificateService, CertificateService>();
=======
            services.AddScoped<IParentService, ParentService>();

>>>>>>> 103a6977b420bef1cbbc6beeffefa4218bd1bafa
            //SignalR
            services.AddSignalR();
            services.AddScoped<INotificationBroadcaster, SignalRNotificationBroadcaster>();
            return services;
        }
    }
}
