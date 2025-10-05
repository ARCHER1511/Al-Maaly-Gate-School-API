using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations
{
    public class StudentSubjectExamConfiguration : IEntityTypeConfiguration<StudentSubjectExam>
    {
        public void Configure(EntityTypeBuilder<StudentSubjectExam> builder)
        {
            builder.HasKey(sse => new { sse.StudentId, sse.SubjectId, sse.ExamId });

            builder.HasOne(sse => sse.Student)
                   .WithMany(s => s.SubjectExams)
                   .HasForeignKey(sse => sse.StudentId);

            builder.HasOne(sse => sse.Subject)
                   .WithMany()
                   .HasForeignKey(sse => sse.SubjectId);

            builder.HasOne(sse => sse.Exam)
                   .WithMany(e => e.StudentSubjectExams)
                   .HasForeignKey(sse => sse.ExamId);
        }
    }
    
}
