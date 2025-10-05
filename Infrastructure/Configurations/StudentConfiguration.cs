using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations
{
    public class StudentConfiguration : IEntityTypeConfiguration<Student>
    {
        public void Configure(EntityTypeBuilder<Student> builder)
        {
            builder.HasOne(s => s.AppUser)
               .WithMany()
               .HasForeignKey(s => s.AppUserId);

            builder.HasOne(s => s.Class)
                   .WithMany(c => c.Students)
                   .HasForeignKey(s => s.ClassId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(s => s.Parents)
                   .WithOne(ps => ps.Student)
                   .HasForeignKey(ps => ps.StudentId);

            builder.HasMany(s => s.SubjectExams)
                   .WithOne(se => se.Student)
                   .HasForeignKey(se => se.StudentId);

            builder.HasMany(s => s.QuestionAnswers)
                   .WithOne(qa => qa.Student)
                   .HasForeignKey(qa => qa.StudentId);
        }
    }
}
