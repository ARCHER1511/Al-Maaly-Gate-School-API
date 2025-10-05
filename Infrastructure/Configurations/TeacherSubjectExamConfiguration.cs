using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace Infrastructure.Configurations
{
    public class TeacherSubjectExamConfiguration : IEntityTypeConfiguration<TeacherSubjectExam>
    {
        public void Configure(EntityTypeBuilder<TeacherSubjectExam> builder)
        {
            builder.HasKey(te => new { te.TeacherId, te.SubjectId, te.ExamId });

            builder.HasOne(te => te.Teacher)
                   .WithMany(t => t.SubjectExams)
                   .HasForeignKey(te => te.TeacherId);

            builder.HasOne(te => te.Subject)
                   .WithMany(s => s.TeacherSubjectExams)
                   .HasForeignKey(te => te.SubjectId);

            builder.HasOne(te => te.Exam)
                   .WithMany(e => e.TeacherSubjectExams)
                   .HasForeignKey(te => te.ExamId);
        }
    }
}
