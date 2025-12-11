using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations
{
    public class ExamQuestionConfiguration : IEntityTypeConfiguration<ExamQuestion>
    {
        public void Configure(EntityTypeBuilder<ExamQuestion> builder)
        {
            builder.ToTable("ExamQuestions", "Academics");

            builder.HasKey(eq => eq.Id);

            // Configure composite unique constraint to prevent duplicate relationships
            builder.HasIndex(eq => new { eq.ExamId, eq.QuestionId })
                   .IsUnique();

            builder.HasOne(eq => eq.Exam)
                   .WithMany(e => e.ExamQuestions)
                   .HasForeignKey(eq => eq.ExamId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(eq => eq.Question)
                   .WithMany(q => q.ExamQuestions)
                   .HasForeignKey(eq => eq.QuestionId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}