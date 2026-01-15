using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations
{
    public class DegreeComponentConfiguration : IEntityTypeConfiguration<DegreeComponent>
    {
        public void Configure(EntityTypeBuilder<DegreeComponent> builder)
        {
            builder.ToTable("DegreeComponents", "Academics");

            builder.HasKey(d => d.Id);

            builder.Property(d => d.ComponentName)
                   .HasMaxLength(100)
                   .IsRequired();

            builder.Property(d => d.Score)
                   .IsRequired()
                   .HasDefaultValue(0);

            builder.Property(d => d.MaxScore)
                   .IsRequired()
                   .HasDefaultValue(0);

            builder.HasOne(d => d.Degree)
                   .WithMany(d => d.Components)
                   .HasForeignKey(d => d.DegreeId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(d => d.ComponentType)
                   .WithMany(c => c.Components)
                   .HasForeignKey(d => d.ComponentTypeId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(d => new { d.DegreeId, d.ComponentTypeId })
                   .IsUnique();
        }
    }

}
