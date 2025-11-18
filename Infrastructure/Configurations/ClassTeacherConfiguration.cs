using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations
{
    public class ClassTeacherConfiguration : IEntityTypeConfiguration<TeacherClass>
    {
        public void Configure(EntityTypeBuilder<TeacherClass> builder)
        {
            builder.HasKey(ct => new { ct.ClassId, ct.TeacherId });

            builder.HasOne(ct => ct.Class)
               .WithMany(c => c.TeacherClasses)
               .HasForeignKey(ct => ct.ClassId);

            builder.HasOne(ct => ct.Teacher)
                   .WithMany(t => t.TeacherClasses)
                   .HasForeignKey(ct => ct.TeacherId);

            builder.ToTable("ClassTeachers", "Academics");
        }
    }
}
