using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations
{
    public class StudentQuestionAnswerExamConfiguration : IEntityTypeConfiguration<StudentQuestionAnswerExam>
    {
        public void Configure(EntityTypeBuilder<StudentQuestionAnswerExam> builder) 
        {
            builder.HasKey(sqae => new { sqae.StudentId, sqae.QuestionId, sqae.AnswerId, sqae.ExamId })
                   .IsClustered(false);

            builder.HasOne(sqae => sqae.Student)
                   .WithMany(s => s.QuestionAnswers)
                   .HasForeignKey(sqae => sqae.StudentId);

            builder.HasOne(sqae => sqae.Question)
                   .WithMany(q => q.StudentQuestionAnswerExam)
                   .HasForeignKey(sqae => sqae.QuestionId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(sqae => sqae.Answer)
                   .WithMany(a => a.StudentQuestionAnswerExam)
                   .HasForeignKey(sqae => sqae.AnswerId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(sqae => sqae.Exam)
                   .WithMany(e => e.StudentQuestionAnswerExams)
                   .HasForeignKey(sqae => sqae.ExamId)
                   .OnDelete(DeleteBehavior.Restrict);

        }
    }
}   
