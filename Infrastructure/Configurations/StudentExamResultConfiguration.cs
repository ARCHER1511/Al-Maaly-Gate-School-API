using Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;


namespace Infrastructure.Configurations
{
    public class StudentExamResultConfiguration : IEntityTypeConfiguration<StudentExamResult>
    {
        public void Configure(EntityTypeBuilder<StudentExamResult> builder)
        {
            builder.ToTable("StudentExamResults", "Academics");

            builder.HasKey(r => r.Id);

            builder.Property(r => r.TotalMark)
                .HasPrecision(10, 2)
                .IsRequired();

            builder.Property(r => r.FullMark)
                .HasPrecision(10, 2)
                .IsRequired();

            builder.Property(r => r.MinMark)
                .HasPrecision(10, 2)
                .IsRequired();

            builder.Property(r => r.Percentage)
                .HasPrecision(5, 2)
                .IsRequired();

            builder.Property(r => r.Status)
                .HasMaxLength(20)
                .IsRequired();

            builder.Property(r => r.StudentId)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(r => r.ExamId)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(r => r.StudentName)
                .HasMaxLength(200);

            builder.Property(r => r.SubjectName)
                .HasMaxLength(200);

            builder.Property(r => r.TeacherName)
                .HasMaxLength(200);

            builder.Property(r => r.ExamName)
                .HasMaxLength(200);

            builder.Property(r => r.Date)
                .HasConversion(
                    v => v.ToDateTime(TimeOnly.MinValue),
                    v => DateOnly.FromDateTime(v)
                )
                .IsRequired();

            builder.HasIndex(r => new { r.StudentId, r.ExamId }).IsUnique();
        }
    }
}
