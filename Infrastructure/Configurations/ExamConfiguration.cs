using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations
{
    public class ExamConfiguration : IEntityTypeConfiguration<Exam>
    {
        public void Configure(EntityTypeBuilder<Exam> builder)
        {
            builder.HasKey(e => e.Id);

            builder.Property(e => e.Type)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.Property(e => e.Link)
                   .HasMaxLength(500);
        }
    }
}
