using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations
{
    public class AnswerConfiguration : IEntityTypeConfiguration<Answer>
    {
        public void Configure(EntityTypeBuilder<Answer> builder)
        {
            builder.HasOne(a => a.Teacher)
                .WithMany(t => t.Answers)
                .HasForeignKey(a => a.TeacherId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(a => a.Question)
                   .WithMany(q => q.Answers)
                   .HasForeignKey(a => a.QuestionId);

            builder.HasMany(a => a.StudentQuestionAnswerExam)
                   .WithOne(sa => sa.Answer)
                   .HasForeignKey(sa => sa.AnswerId);
        }
    }
   
}
