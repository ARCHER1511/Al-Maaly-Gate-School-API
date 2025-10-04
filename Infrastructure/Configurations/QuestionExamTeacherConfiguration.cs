using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations
{
    public class QuestionExamTeacherConfiguration : IEntityTypeConfiguration<QuestionExamTeacher>
    {
        public void Configure(EntityTypeBuilder<QuestionExamTeacher> builder)
        {
            
        }
    }
}
