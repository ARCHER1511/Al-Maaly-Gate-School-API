using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations
{
    public class StudentExamAnswerConfiguration : IEntityTypeConfiguration<StudentExamAnswer>
    {
        public void Configure(EntityTypeBuilder<StudentExamAnswer> builder)
        {
            builder.ToTable("StudentExamAnswers", "Academics");


            builder.ToTable("StudentExamAnswers", "Academics");

            builder.HasKey(a => a.Id);

            builder.Property(a => a.Mark)
                   .HasColumnType("decimal(18,2)");

            builder.Property(a => a.FullName)
                   .IsRequired();

            builder.HasOne(a => a.Student)
                   .WithMany()
                   .HasForeignKey(a => a.StudentId)
                   .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(a => a.Exam)
                   .WithMany()
                   .HasForeignKey(a => a.ExamId)
                   .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(a => a.Question)
                   .WithMany()
                   .HasForeignKey(a => a.QuestionId)
                   .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(a => a.Choice)
                   .WithMany()
                   .HasForeignKey(a => a.ChoiceId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(a => a.TrueAndFalse)
                   .WithMany()
                   .HasForeignKey(a => a.TrueAndFalseId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(a => a.TextAnswer)
                   .WithMany()
                   .HasForeignKey(a => a.TextAnswerId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(a => new { a.StudentId, a.ExamId, a.QuestionId })
                   .IsUnique();
        }
    }
}
