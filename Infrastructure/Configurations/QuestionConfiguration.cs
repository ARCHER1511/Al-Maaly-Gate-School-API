using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations
{
    public class QuestionConfiguration : IEntityTypeConfiguration<Question>
    {
        public void Configure(EntityTypeBuilder<Question> builder) 
        {
            builder.HasOne(q => q.Teacher)
               .WithMany(t => t.Questions)
               .HasForeignKey(q => q.TeacherId);

            builder.HasMany(q => q.QuestionExamTeachers)
                   .WithOne(qe => qe.Question)
                   .HasForeignKey(qe => qe.QuestionId);

            builder.HasMany(q => q.Answers)
                   .WithOne(a => a.Question)
                   .HasForeignKey(a => a.QuestionId);

            builder.HasMany(q => q.StudentQuestionAnswerExam)
                   .WithOne(sa => sa.Question)
                   .HasForeignKey(sa => sa.QuestionId);
        }
    }
}
