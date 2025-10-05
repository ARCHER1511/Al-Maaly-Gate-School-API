using Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace Infrastructure.Data
{
    public class AlMaalyGateSchoolContext : IdentityDbContext<AppUser, AppRole, string>
    {
        public AlMaalyGateSchoolContext(DbContextOptions<AlMaalyGateSchoolContext> options)
            : base(options)
        {
            
        }
        public DbSet<Admin> Admins { get; set; }
        public DbSet<Answer> Answers { get; set; }
        public DbSet<Class> Classes { get; set; }
        public DbSet<ClassAppointment> ClassAppointments { get; set; }
        public DbSet<ClassAssets> ClassAssets { get; set; }
        public DbSet<ClassSubject> ClassSubjects { get; set; }
        public DbSet<Exam> Exams { get; set; }
        public DbSet<Parent> Parents { get; set; }
        public DbSet<ParentStudent> ParentStudents { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<QuestionExamTeacher> QuestionExamTeachers { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<StudentQuestionAnswerExam> StudentQuestionAnswerExams { get; set; }
        public DbSet<StudentSubjectExam> StudentSubjectExams { get; set; }
        public DbSet<Subject> Subjects { get; set; }
        public DbSet<Teacher> Teachers { get; set; }
        public DbSet<TeacherSubjectExam> TeacherSubjectExams { get; set; }

        //On model creating with configuration and Factory On using onion architecture
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }
    }
}

