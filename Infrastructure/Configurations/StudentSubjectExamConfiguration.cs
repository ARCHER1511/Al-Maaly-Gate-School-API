using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations
{
    public class StudentSubjectExamConfiguration : IEntityTypeConfiguration<StudentSubjectExam>
    {
        public void Configure(EntityTypeBuilder<StudentSubjectExam> builder)
        {
            
        }
    }
    
}
