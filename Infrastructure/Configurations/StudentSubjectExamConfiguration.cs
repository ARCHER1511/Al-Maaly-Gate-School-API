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

            builder.Property(sse => sse.Grade)
                   .HasColumnType("decimal(5,2)");

            builder.HasOne(sse => sse.Student)
                   .WithMany(s => s.SubjectExams)
                   .HasForeignKey(sse => sse.StudentId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(sse => sse.Subject)
                   .WithMany(s => s.StudentSubjectExams)
                   .HasForeignKey(sse => sse.SubjectId);

            builder.HasOne(sse => sse.Exam)
                   .WithMany(e => e.StudentSubjectExams)
                   .HasForeignKey(sse => sse.ExamId);

            builder.ToTable("StudentSubjectExams", "Academics");
        }
    }
}
