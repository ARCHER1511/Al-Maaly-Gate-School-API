using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations
{
    public class ClassConfiguration : IEntityTypeConfiguration<Class>
    {
        public void Configure(EntityTypeBuilder<Class> builder)
        {
            new BaseEntityConfiguration<Class>().Configure(builder);

            builder.Property(c => c.ClassYear)
                   .IsRequired()
                   .HasMaxLength(50);

            builder.HasOne(c => c.Teacher)
                   .WithMany(t => t.Classes)
                   .HasForeignKey(c => c.TeacherId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(c => c.Students)
                   .WithOne(s => s.Class)
                   .HasForeignKey(s => s.ClassId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.ToTable("Classes", "Academics");
        }
    }
}
