using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations
{
    public class ClassConfiguration : IEntityTypeConfiguration<Class>
    {
        public void Configure(EntityTypeBuilder<Class> builder)
        {
            builder.ToTable("Classes", "Academics");

            builder.HasKey(c => c.Id);

            builder.Property(c => c.ClassName)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.HasOne(c => c.Grade)
                   .WithMany(g => g.Classes)
                   .HasForeignKey(c => c.GradeId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(c => c.ClassAssets)
                   .WithOne(ca => ca.Class)
                   .HasForeignKey(ca => ca.ClassId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(c => c.ClassAppointments)
                   .WithOne(ca => ca.Class)
                   .HasForeignKey(ca => ca.ClassId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
