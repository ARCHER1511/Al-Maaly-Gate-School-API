using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace Infrastructure.Configurations
{
    public class TeacherSubjectExamConfiguration : IEntityTypeConfiguration<TeacherSubjectExam>
    {
        public void Configure(EntityTypeBuilder<TeacherSubjectExam> builder)
        {
            
        }
    }
}
