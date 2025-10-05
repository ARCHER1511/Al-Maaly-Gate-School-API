using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations
{
    public class ClassConfiguration : IEntityTypeConfiguration<Class>
    {
        public void Configure(EntityTypeBuilder<Class> builder) 
        {
            builder.HasOne(c => c.Teacher)
               .WithMany(t => t.Classes)
               .HasForeignKey(c => c.TeacherId);

            builder.HasMany(c => c.ClassSubjects)
                   .WithOne(cs => cs.Class)
                   .HasForeignKey(cs => cs.ClassId);

            builder.HasMany(c => c.ClassAssets)
                   .WithOne(a => a.Class)
                   .HasForeignKey(a => a.ClassId);

            builder.HasMany(c => c.ClassAppointments)
                   .WithOne(a => a.Class)
                   .HasForeignKey(a => a.ClassId);
        }
    }
}
