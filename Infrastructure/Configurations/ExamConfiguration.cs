using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Configurations
{
    public class ExamConfiguration : IEntityTypeConfiguration<Exam>
    {
        public void Configure(EntityTypeBuilder<Exam> builder) 
        {
            builder.HasMany(e => e.TeacherSubjectExams)
               .WithOne(te => te.Exam)
               .HasForeignKey(te => te.ExamId);

            builder.HasMany(e => e.StudentSubjectExams)
                   .WithOne(se => se.Exam)
                   .HasForeignKey(se => se.ExamId);

            builder.HasMany(e => e.QuestionExamTeachers)
                   .WithOne(qe => qe.Exam)
                   .HasForeignKey(qe => qe.ExamId);

            builder.HasMany(e => e.StudentQuestionAnswerExams)
                   .WithOne(sa => sa.Exam)
                   .HasForeignKey(sa => sa.ExamId);
        }
    }
}
