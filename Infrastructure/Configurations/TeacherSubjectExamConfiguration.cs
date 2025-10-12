using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace Infrastructure.Configurations
{
    public class TeacherSubjectExamConfiguration : IEntityTypeConfiguration<TeacherSubjectExam>
    {
        public void Configure(EntityTypeBuilder<TeacherSubjectExam> builder)
        {
            builder.HasKey(tse => new { tse.TeacherId, tse.SubjectId, tse.ExamId });

            builder.HasOne(tse => tse.Teacher)
                   .WithMany(t => t.SubjectExams)
                   .HasForeignKey(tse => tse.TeacherId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(tse => tse.Subject)
                   .WithMany(s => s.TeacherSubjectExams)
                   .HasForeignKey(tse => tse.SubjectId)
                    .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(tse => tse.Exam)
                   .WithMany(e => e.TeacherSubjectExams)
                   .HasForeignKey(tse => tse.ExamId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.ToTable("TeacherSubjectExams", "Academics");
        }
    }
}
