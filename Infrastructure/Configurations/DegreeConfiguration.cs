using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Configurations
{
    public class DegreeConfiguration : IEntityTypeConfiguration<Degree>
    {
        public void Configure(EntityTypeBuilder<Degree> builder)
        {
            builder.ToTable("Degrees", "Academics");

            builder.HasKey(d => d.Id);

            builder.Property(d => d.SubjectName)
                   .HasMaxLength(150);

            builder.Property(d => d.Score).IsRequired();
            builder.Property(d => d.MaxScore).IsRequired();
            builder.Property(d => d.DegreeType).IsRequired();

            builder.HasOne(d => d.Student)
                   .WithMany(s => s.Degrees)
                   .HasForeignKey(d => d.StudentId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(d => d.Subject)
                   .WithMany(s => s.Degrees)
                   .HasForeignKey(d => d.SubjectId)
                   .OnDelete(DeleteBehavior.Restrict);

            // Add relationship to components
            builder.HasMany(d => d.Components)
                   .WithOne(c => c.Degree)
                   .HasForeignKey(c => c.DegreeId);
        }
    }
}
