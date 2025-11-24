using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations
{
    public class SubjectConfiguration : IEntityTypeConfiguration<Subject>
    {
        public void Configure(EntityTypeBuilder<Subject> builder)
        {
            builder.HasKey(s => s.Id);

            builder.Property(s => s.ClassYear)
                   .IsRequired()
                   .HasMaxLength(50);

            builder.HasOne(s => s.Class)
                   .WithMany(c => c.Subjects)
                   .HasForeignKey(s => s.ClassId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(s => s.Exams)
                   .WithOne(e => e.Subject)
                   .HasForeignKey(e => e.SubjectId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(s => s.ClassAppointments)
                   .WithOne(e => e.Subject)
                   .HasForeignKey(e => e.SubjectId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.Property(s => s.SubjectName)
                   .IsRequired()
                   .HasMaxLength(150);

            builder.Property(s => s.CreditHours)
                   .IsRequired()
                   .HasDefaultValue(3.0);

            builder.ToTable("Subjects", "Academics");
        }
    }

}
