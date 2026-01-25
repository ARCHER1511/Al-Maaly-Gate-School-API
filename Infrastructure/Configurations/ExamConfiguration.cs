using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations
{
    public class ExamConfiguration : IEntityTypeConfiguration<Exam>
    {
        public void Configure(EntityTypeBuilder<Exam> builder)
        {
            builder.ToTable("Exams", "Academics");

            builder.HasKey(e => e.Id);

            builder.HasOne(e => e.Subject)
                   .WithMany(s => s.Exams)
                   .HasForeignKey(e => e.SubjectId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(e => e.Teacher)
                   .WithMany(t => t.Exams)
                   .HasForeignKey(e => e.TeacherId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(e => e.Class)
                   .WithMany(c => c.Exams)
                   .HasForeignKey(e => e.ClassId)
                   .OnDelete(DeleteBehavior.Restrict);

            // Remove the old one-to-many configuration
            // builder.HasMany(e => e.Questions)
            //        .WithOne(q => q.Exam)
            //        .HasForeignKey(q => q.ExamId)
            //        .OnDelete(DeleteBehavior.NoAction);

            // Configure the many-to-many relationship through ExamQuestion
            builder.HasMany(e => e.ExamQuestions)
                   .WithOne(eq => eq.Exam)
                   .HasForeignKey(eq => eq.ExamId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.Property(e => e.MinMark)
                   .HasPrecision(5, 2);

            builder.Property(e => e.FullMark)
                   .HasPrecision(5, 2);

            builder.Property(e => e.Status)
                   .HasMaxLength(20)
                   .HasDefaultValue("Upcoming");

            builder.Property(e => e.Start)
                   .HasColumnType("datetimeoffset");

            builder.Property(e => e.End)
                   .HasColumnType("datetimeoffset");
        }
    }
}