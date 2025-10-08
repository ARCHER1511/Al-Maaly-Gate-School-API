using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations
{
    public class SubjectConfiguration : IEntityTypeConfiguration<Subject>
    {
        public void Configure(EntityTypeBuilder<Subject> builder)
        {
            new BaseEntityConfiguration<Subject>().Configure(builder);

            builder.Property(s => s.ClassYear)
                   .IsRequired()
                   .HasMaxLength(50);
        }
    }

}
