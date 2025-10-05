using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations
{
    public class QuestionExamTeacherConfiguration : IEntityTypeConfiguration<QuestionExamTeacher>
    {
        public void Configure(EntityTypeBuilder<QuestionExamTeacher> builder)
        {
            builder.HasKey(qe => new { qe.QuestionId, qe.ExamId, qe.TeacherId })
                   .IsClustered(false);

            builder.HasOne(qe => qe.Question)
                   .WithMany(q => q.QuestionExamTeachers)
                   .HasForeignKey(qe => qe.QuestionId);

            builder.HasOne(qe => qe.Exam)
                   .WithMany(e => e.QuestionExamTeachers)
                   .HasForeignKey(qe => qe.ExamId);

            builder.HasOne(qe => qe.Teacher)
                   .WithMany(t => t.QuestionExams)
                   .HasForeignKey(qe => qe.TeacherId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
