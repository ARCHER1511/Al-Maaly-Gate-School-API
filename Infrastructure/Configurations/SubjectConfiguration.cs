using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations
{
    public class SubjectConfiguration : IEntityTypeConfiguration<Subject>
    {
        public void Configure(EntityTypeBuilder<Subject> builder)
        {
            builder.HasMany(s => s.ClassSubjects)
               .WithOne(cs => cs.Subject)   
               .HasForeignKey(cs => cs.SubjectId);

            builder.HasMany(s => s.TeacherSubjectExams)
                   .WithOne(te => te.Subject)
                   .HasForeignKey(te => te.SubjectId);
        }
    }
    
}
