using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations
{
    public class DegreeComponentTypeConfiguration : IEntityTypeConfiguration<DegreeComponentType>
    {
        public void Configure(EntityTypeBuilder<DegreeComponentType> builder)
        {
            builder.ToTable("DegreeComponentTypes", "Academics");

            builder.HasKey(d => d.Id);

            builder.Property(d => d.ComponentName)
                   .HasMaxLength(100)
                   .IsRequired();

            builder.Property(d => d.MaxScore)
                   .IsRequired()
                   .HasDefaultValue(0);

            builder.Property(d => d.Order)
                   .IsRequired()
                   .HasDefaultValue(1);

            builder.Property(d => d.IsActive)
                   .IsRequired()
                   .HasDefaultValue(true);

            builder.HasOne(d => d.Subject)
                   .WithMany(s => s.ComponentTypes)
                   .HasForeignKey(d => d.SubjectId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(d => new { d.SubjectId, d.ComponentName })
                   .IsUnique();
        }
    }
}
