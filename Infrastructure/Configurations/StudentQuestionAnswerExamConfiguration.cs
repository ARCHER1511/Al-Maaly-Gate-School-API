using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations
{
    public class StudentQuestionAnswerExamConfiguration : IEntityTypeConfiguration<StudentQuestionAnswerExam>
    {
        public void Configure(EntityTypeBuilder<StudentQuestionAnswerExam> builder) 
        {

        }
    }
}
