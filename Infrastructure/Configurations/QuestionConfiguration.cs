using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations
{
    public class QuestionConfiguration : IEntityTypeConfiguration<Question>
    {
        public void Configure(EntityTypeBuilder<Question> builder)
        {
            builder.ToTable("Questions", "Academics");

            builder.HasOne(q => q.Exam)
                   .WithMany(e => e.Questions)
                   .HasForeignKey(q => q.ExamId)
                   .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(q => q.Teacher)
                  .WithMany(e => e.Questions)
                  .HasForeignKey(q => q.TeacherId)
                  .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(q => q.Choices)
                   .WithOne(c => c.Question)
                   .HasForeignKey(c => c.QuestionId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(q => q.ChoiceAnswer)
                   .WithOne(ca => ca.Question)
                   .HasForeignKey<ChoiceAnswer>(ca => ca.QuestionId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.Property(q => q.Degree).HasPrecision(5, 2);

        }
    }
}
