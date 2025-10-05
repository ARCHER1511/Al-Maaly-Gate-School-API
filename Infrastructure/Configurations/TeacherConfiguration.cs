using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations
{
    public class TeacherConfiguration : IEntityTypeConfiguration<Teacher>
    {
        public void Configure(EntityTypeBuilder<Teacher> builder)
        {
            builder.HasOne(t => t.AppUser)
               .WithMany()
               .HasForeignKey(t => t.AppUserId);

            builder.HasMany(t => t.Classes)
                   .WithOne(c => c.Teacher)
                   .HasForeignKey(c => c.TeacherId);

            builder.HasMany(t => t.SubjectExams)
                   .WithOne(se => se.Teacher)
                   .HasForeignKey(se => se.TeacherId);

            builder.HasMany(t => t.QuestionExams)
                   .WithOne(qe => qe.Teacher)
                   .HasForeignKey(qe => qe.TeacherId);

            builder.HasMany(t => t.Questions)
                   .WithOne(q => q.Teacher)
                   .HasForeignKey(q => q.TeacherId);

            builder.HasMany(t => t.Answers)
                   .WithOne(a => a.Teacher)
                   .HasForeignKey(a => a.TeacherId);
        }
    }
    
}
