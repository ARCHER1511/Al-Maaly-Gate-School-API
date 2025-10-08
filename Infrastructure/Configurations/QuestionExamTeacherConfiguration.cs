using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations
{
    public class QuestionExamTeacherConfiguration : IEntityTypeConfiguration<QuestionExamTeacher>
    {
        public void Configure(EntityTypeBuilder<QuestionExamTeacher> builder)
        {
            builder.HasKey(qet => new { qet.QuestionId, qet.TeacherId, qet.ExamId });

            builder.HasOne(qet => qet.Question)
                   .WithMany(q => q.QuestionExamTeachers)
                   .HasForeignKey(qet => qet.QuestionId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(qet => qet.Teacher)
                   .WithMany(t => t.QuestionExams)
                   .HasForeignKey(qet => qet.TeacherId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(qet => qet.Exam)
                   .WithMany(e => e.QuestionExamTeachers)
                   .HasForeignKey(qet => qet.ExamId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
