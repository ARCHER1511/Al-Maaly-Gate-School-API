using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations
{
    public class TeacherSubjectConfiguration : IEntityTypeConfiguration<TeacherSubject>
    {
        public void Configure(EntityTypeBuilder<TeacherSubject> builder)
        {
            builder.ToTable("TeacherSubjects", "Academics");

            builder.HasKey(ts => ts.Id);

            builder.HasIndex(ts => new { ts.TeacherId, ts.SubjectId })
                   .IsUnique();

            builder.HasOne(ts => ts.Teacher)
                .WithMany(t => t.TeacherSubjects)
                .HasForeignKey(ts => ts.TeacherId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(ts => ts.Subject)
                .WithMany(s => s.TeacherSubjects)
                .HasForeignKey(ts => ts.SubjectId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }

}
