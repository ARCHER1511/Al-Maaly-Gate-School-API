using System.Reflection;
using Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data
{
    public class AlMaalyGateSchoolContext
        : IdentityDbContext<AppUser, AppRole, string,
            IdentityUserClaim<string>,
            AppUserRole,
            IdentityUserLogin<string>,
            IdentityRoleClaim<string>,
            IdentityUserToken<string>>
    {
        public AlMaalyGateSchoolContext(DbContextOptions<AlMaalyGateSchoolContext> options)
            : base(options) { }

        public DbSet<Admin> Admins { get; set; }
        public DbSet<Class> Classes { get; set; }
        public DbSet<AppUserRole> AppUserRoles { get; set; }
        public DbSet<ClassAppointment> ClassAppointments { get; set; }
        public DbSet<ClassAssets> ClassAssets { get; set; }
        public DbSet<Exam> Exams { get; set; }
        public DbSet<FileRecord> FileRecords { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<Parent> Parents { get; set; }
        public DbSet<ParentStudent> ParentStudents { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<Subject> Subjects { get; set; }
        public DbSet<Teacher> Teachers { get; set; }
        public DbSet<UserNotification> UserNotifications { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<Choices> Choices { get; set; }
        public DbSet<StudentExamAnswer> StudentExamAnswer { get; set; }
        public DbSet<TeacherClass> TeacherClasses { get; set; }
        public DbSet<TeacherSubject> TeacherSubjects { get; set; }
        public DbSet<Degree> Degrees { get; set; }
        public DbSet<Certificate> Certificates { get; set; }
        public DbSet<Grade> Grades { get; set; }

        //On model creating with configuration and Factory On using onion architecture
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }
    }
}
